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
        public async Task Test_01_RealtimeSession()
        {
            RealtimeSession session = null;

            try
            {
                Assert.IsNotNull(OpenAIClient.RealtimeEndpoint);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

                var tools = new List<Tool>
                {
                    Tool.FromFunc("goodbye", () =>
                    {
                        cts.Cancel();
                        return "Goodbye!";
                    })
                };

                var options = new Options(Model.GPT4oRealtime, tools: tools);
                session = await OpenAIClient.RealtimeEndpoint.CreateSessionAsync(options, cts.Token);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.Options);
                Assert.AreEqual(Model.GPT4oRealtime.Id, options.Model);
                Assert.AreEqual(options.Model, session.Options.Model);
                Assert.IsNotNull(options.Tools);
                Assert.IsNotEmpty(options.Tools);
                Assert.AreEqual(1, options.Tools.Count);
                Assert.AreEqual(options.Tools.Count, session.Options.Tools.Count);
                Assert.AreEqual(options.Tools[0].Name, session.Options.Tools[0].Name);
                Assert.AreEqual(Modality.Audio | Modality.Text, options.Modalities);
                Assert.AreEqual(Modality.Audio | Modality.Text, session.Options.Modalities);
                var responseTask = session.ReceiveUpdatesAsync<IServerEvent>(SessionEvents, cts.Token);

                await session.SendAsync(new ConversationItemCreateRequest("Hello!"), cts.Token);
                await session.SendAsync(new CreateResponseRequest(), cts.Token);
                await Task.Delay(5000, cts.Token).ConfigureAwait(true);
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
                                toolCall.InvokeFunction();
                            }

                            break;
                    }
                }

                await responseTask.ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
}
