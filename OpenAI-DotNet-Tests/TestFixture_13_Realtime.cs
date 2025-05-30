// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Realtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_13_Realtime : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_01_RealtimeSession()
        {
            RealtimeSession session = null;

            try
            {
                Assert.IsNotNull(OpenAIClient.RealtimeEndpoint);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                var wasGoodbyeCalled = false;
                var mutex = new object();
                var tools = new List<Tool>
                {
                    Tool.FromFunc("goodbye", () =>
                    {
                        lock (mutex)
                        {
                            wasGoodbyeCalled = true;
                        }
                        Console.WriteLine("Hanging up...");
                        cts.Cancel();
                        return "Goodbye!";
                    })
                };

                var configuration = new SessionConfiguration(Model.GPT4oRealtime, tools: tools);
                session = await OpenAIClient.RealtimeEndpoint.CreateSessionAsync(configuration, cts.Token);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.Configuration);
                Assert.AreEqual(Model.GPT4oRealtime.Id, configuration.Model);
                Assert.AreEqual(configuration.Model, session.Configuration.Model);
                Assert.IsNotNull(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsInstanceOf<ServerVAD>(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsNotNull(configuration.Tools);
                Assert.IsNotEmpty(configuration.Tools);
                Assert.AreEqual(1, configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools.Count, session.Configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools[0].Name, session.Configuration.Tools[0].Name);
                Assert.AreEqual(Modality.Audio | Modality.Text, configuration.Modalities);
                Assert.AreEqual(Modality.Audio | Modality.Text, session.Configuration.Modalities);
                var responseTask = session.ReceiveUpdatesAsync<IServerEvent>(SessionEvents, cts.Token);

                await session.SendAsync(new ConversationItemCreateRequest("Hello!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);
                await session.SendAsync(new InputAudioBufferAppendRequest(new ReadOnlyMemory<byte>(new byte[1024 * 4])), cts.Token);
                await session.SendAsync(new ConversationItemCreateRequest("Goodbye!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);

                void SessionEvents(IServerEvent @event)
                {
                    switch (@event)
                    {
                        case ResponseAudioTranscriptResponse transcriptResponse:
                            Console.WriteLine(transcriptResponse.ToString());
                            break;
                        case ResponseFunctionCallArgumentsResponse functionCallResponse:
                            if (functionCallResponse.IsDone)
                            {
                                ToolCall toolCall = functionCallResponse;
                                Console.WriteLine($"tool_call: {toolCall.Function.Name}");
                                toolCall.InvokeFunction();
                            }

                            break;
                    }
                }

                await responseTask.ConfigureAwait(true);

                lock (mutex)
                {
                    Assert.IsTrue(wasGoodbyeCalled);
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ObjectDisposedException:
                        // ignore
                        break;
                    default:
                        Console.WriteLine(e);
                        throw;
                }
            }
            finally
            {
                session?.Dispose();
            }
        }

        [Test]
        public async Task Test_01_02_RealtimeSession_IAsyncEnumerable()
        {
            RealtimeSession session = null;

            try
            {
                Assert.IsNotNull(OpenAIClient.RealtimeEndpoint);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                var wasGoodbyeCalled = false;
                var mutex = new object();
                var tools = new List<Tool>
                {
                    Tool.FromFunc("goodbye", () =>
                    {
                        lock (mutex)
                        {
                            wasGoodbyeCalled = true;
                        }
                        Console.WriteLine("Hanging up...");
                        cts.Cancel();
                        return "Goodbye!";
                    })
                };

                var configuration = new SessionConfiguration(Model.GPT4oRealtime, tools: tools);
                session = await OpenAIClient.RealtimeEndpoint.CreateSessionAsync(configuration, cts.Token);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.Configuration);
                Assert.AreEqual(Model.GPT4oRealtime.Id, configuration.Model);
                Assert.AreEqual(configuration.Model, session.Configuration.Model);
                Assert.IsNotNull(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsInstanceOf<ServerVAD>(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsNotNull(configuration.Tools);
                Assert.IsNotEmpty(configuration.Tools);
                Assert.AreEqual(1, configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools.Count, session.Configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools[0].Name, session.Configuration.Tools[0].Name);
                Assert.AreEqual(Modality.Audio | Modality.Text, configuration.Modalities);
                Assert.AreEqual(Modality.Audio | Modality.Text, session.Configuration.Modalities);

                await foreach (var @event in session.ReceiveUpdatesAsync<IServerEvent>(cts.Token).ConfigureAwait(false))
                {
                    switch (@event)
                    {
                        case ConversationItemCreatedResponse:
                            Console.WriteLine("conversation created");
                            break;
                        case SessionResponse sessionResponse:
                            if (sessionResponse.Type != "session.created") { return; }
                            await session.SendAsync(new ConversationItemCreateRequest("Hello!"), cts.Token);
                            await session.SendAsync(new CreateResponseRequest(), cts.Token);
                            await session.SendAsync(new InputAudioBufferAppendRequest(new ReadOnlyMemory<byte>(new byte[1024 * 4])), cts.Token);
                            await session.SendAsync(new ConversationItemCreateRequest("Goodbye!"), cts.Token);
                            await session.SendAsync(new CreateResponseRequest(), cts.Token);
                            break;
                        case ResponseAudioTranscriptResponse transcriptResponse:
                            Console.WriteLine(transcriptResponse.ToString());
                            break;
                        case ResponseFunctionCallArgumentsResponse functionCallResponse:
                            if (functionCallResponse.IsDone)
                            {
                                ToolCall toolCall = functionCallResponse;
                                Console.WriteLine($"tool_call: {toolCall.Function.Name}");
                                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                                toolCall.InvokeFunction();
                            }

                            break;
                    }
                }

                lock (mutex)
                {
                    Assert.IsTrue(wasGoodbyeCalled);
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ObjectDisposedException:
                        // ignore
                        break;
                    default:
                        Console.WriteLine(e);
                        throw;
                }
            }
            finally
            {
                session?.Dispose();
            }
        }

        [Test]
        public async Task Test_02_RealtimeSession_Semantic_VAD()
        {
            RealtimeSession session = null;

            try
            {
                Assert.IsNotNull(OpenAIClient.RealtimeEndpoint);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                var wasGoodbyeCalled = false;
                var mutex = new object();
                var tools = new List<Tool>
                {
                    Tool.FromFunc("goodbye", () =>
                    {
                        lock (mutex)
                        {
                            wasGoodbyeCalled = true;
                        }
                        Console.WriteLine("Hanging up...");
                        cts.Cancel();
                        return "Goodbye!";
                    })
                };

                var configuration = new SessionConfiguration(
                    model: Model.GPT4oRealtime,
                    turnDetectionSettings: new SemanticVAD(),
                    tools: tools);
                session = await OpenAIClient.RealtimeEndpoint.CreateSessionAsync(configuration, cts.Token);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.Configuration);
                Assert.AreEqual(Model.GPT4oRealtime.Id, configuration.Model);
                Assert.AreEqual(configuration.Model, session.Configuration.Model);
                Assert.IsNotNull(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsInstanceOf<SemanticVAD>(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsNotNull(configuration.Tools);
                Assert.IsNotEmpty(configuration.Tools);
                Assert.AreEqual(1, configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools.Count, session.Configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools[0].Name, session.Configuration.Tools[0].Name);
                Assert.AreEqual(Modality.Audio | Modality.Text, configuration.Modalities);
                Assert.AreEqual(Modality.Audio | Modality.Text, session.Configuration.Modalities);
                var responseTask = session.ReceiveUpdatesAsync<IServerEvent>(SessionEvents, cts.Token);

                await session.SendAsync(new ConversationItemCreateRequest("Hello!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);
                await session.SendAsync(new InputAudioBufferAppendRequest(new ReadOnlyMemory<byte>(new byte[1024 * 4])), cts.Token);
                await session.SendAsync(new ConversationItemCreateRequest("Goodbye!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);

                void SessionEvents(IServerEvent @event)
                {
                    switch (@event)
                    {
                        case ResponseAudioTranscriptResponse transcriptResponse:
                            Console.WriteLine(transcriptResponse.ToString());
                            break;
                        case ResponseFunctionCallArgumentsResponse functionCallResponse:
                            if (functionCallResponse.IsDone)
                            {
                                ToolCall toolCall = functionCallResponse;
                                Console.WriteLine($"tool_call: {toolCall.Function.Name}");
                                toolCall.InvokeFunction();
                            }

                            break;
                    }
                }

                await responseTask.ConfigureAwait(true);

                lock (mutex)
                {
                    Assert.IsTrue(wasGoodbyeCalled);
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ObjectDisposedException:
                        // ignore
                        break;
                    default:
                        Console.WriteLine(e);
                        throw;
                }
            }
            finally
            {
                session?.Dispose();
            }
        }

        [Test]
        public async Task Test_03_RealtimeSession_VAD_Disabled()
        {
            RealtimeSession session = null;

            try
            {
                Assert.IsNotNull(OpenAIClient.RealtimeEndpoint);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                var wasGoodbyeCalled = false;
                var mutex = new object();
                var tools = new List<Tool>
                {
                    Tool.FromFunc("goodbye", () =>
                    {
                        lock (mutex)
                        {
                            wasGoodbyeCalled = true;
                        }
                        Console.WriteLine("Hanging up...");
                        cts.Cancel();
                        return "Goodbye!";
                    })
                };

                var configuration = new SessionConfiguration(
                    model: Model.GPT4oRealtime,
                    tools: tools,
                    modalities: Modality.Text,
                    turnDetectionSettings: new DisabledVAD());
                session = await OpenAIClient.RealtimeEndpoint.CreateSessionAsync(configuration, cts.Token);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.Configuration);
                Assert.AreEqual(Model.GPT4oRealtime.Id, configuration.Model);
                Assert.AreEqual(configuration.Model, session.Configuration.Model);
                Assert.IsNull(session.Configuration.VoiceActivityDetectionSettings);
                Assert.IsNotNull(configuration.Tools);
                Assert.IsNotEmpty(configuration.Tools);
                Assert.AreEqual(1, configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools.Count, session.Configuration.Tools.Count);
                Assert.AreEqual(configuration.Tools[0].Name, session.Configuration.Tools[0].Name);
                Assert.AreEqual(Modality.Text, configuration.Modalities);
                Assert.AreEqual(Modality.Text, session.Configuration.Modalities);
                var responseTask = session.ReceiveUpdatesAsync<IServerEvent>(SessionEvents, cts.Token);

                await session.SendAsync(new ConversationItemCreateRequest("Hello!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);
                await session.SendAsync(new InputAudioBufferAppendRequest(new ReadOnlyMemory<byte>(new byte[1024 * 8])), cts.Token);
                await session.SendAsync(new InputAudioBufferCommitRequest(), cts.Token);
                await session.SendAsync(new ConversationItemCreateRequest("Goodbye!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);

                void SessionEvents(IServerEvent @event)
                {
                    switch (@event)
                    {
                        case ResponseAudioTranscriptResponse transcriptResponse:
                            Console.WriteLine(transcriptResponse.ToString());
                            break;
                        case ResponseFunctionCallArgumentsResponse functionCallResponse:
                            if (functionCallResponse.IsDone)
                            {
                                ToolCall toolCall = functionCallResponse;
                                Console.WriteLine($"tool_call: {toolCall.Function.Name}");
                                toolCall.InvokeFunction();
                            }

                            break;
                    }
                }

                await responseTask.ConfigureAwait(true);

                lock (mutex)
                {
                    Assert.IsTrue(wasGoodbyeCalled);
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ObjectDisposedException:
                        // ignore
                        break;
                    default:
                        Console.WriteLine(e);
                        throw;
                }
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
}
