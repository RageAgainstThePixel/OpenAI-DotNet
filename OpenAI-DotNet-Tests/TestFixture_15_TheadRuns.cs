using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.ThreadRuns;
using OpenAI.Threads;

namespace OpenAI.Tests
{
    internal class TestFixture_15_TheadRuns : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_CreateThreadRun()
        {
            Assert.NotNull(OpenAIClient.ThreadRunsEndpoint);

            var createAssistantRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(createAssistantRequest);

            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var request = new CreateThreadRunRequest(assistant.Id)
            {
                Instructions = "Run test instructions",
                Model = "gpt-3.5-turbo",
                Metadata = new Dictionary<string, string>
                {
                    ["key"] = "value"
                }
            };

            var run = await OpenAIClient.ThreadRunsEndpoint.CreateThreadRunAsync(thread.Id, request);

            Assert.IsNotNull(run);
            Assert.AreEqual("gpt-3.5-turbo", run.Model);
            Assert.AreEqual("Run test instructions", run.Instructions);

            Assert.IsNotNull(run.Metadata);
            Assert.Contains("key", run.Metadata.Keys.ToList());
            Assert.AreEqual("value", run.Metadata["key"]);
        }

        [Test]
        public async Task Test_02_CreateThreadAndRun()
        {
            var createAssistantRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(createAssistantRequest);

            var request = new CreateThreadAndRunRequest(assistant.Id)
            {
                Thread = new CreateThreadAndRunRequest.ThreadForRun
                {
                    Messages = new List<CreateThreadRequest.Message>
                    {
                        new("Thread message text")
                    }
                },
                Instructions = "Run test instructions",
                Model = "gpt-3.5-turbo",
                Metadata = new Dictionary<string, string>
                {
                    ["key"] = "value"
                }
            };

            var run = await OpenAIClient.ThreadRunsEndpoint.CreateThreadAndRunAsync(request);

            Assert.IsNotNull(run);
            Assert.AreEqual("gpt-3.5-turbo", run.Model);
            Assert.AreEqual("Run test instructions", run.Instructions);

            Assert.IsNotNull(run.Metadata);
            Assert.Contains("key", run.Metadata.Keys.ToList());
            Assert.AreEqual("value", run.Metadata["key"]);

            Assert.IsNotNull(run.ThreadId);
        }

        [Test]
        public async Task Test_03_ListThreadRuns()
        {
            var createAssistantRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(createAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var request = new CreateThreadRunRequest(assistant.Id);
            var run = await OpenAIClient.ThreadRunsEndpoint.CreateThreadRunAsync(thread.Id, request);

            var list = await OpenAIClient.ThreadRunsEndpoint.ListThreadRunsAsync(thread.Id);

            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Data);

            foreach (var threadRun in list.Data)
            {
                var retrieved =
                    await OpenAIClient.ThreadRunsEndpoint.RetrieveRunAsync(threadRun.ThreadId, threadRun.Id);

                Assert.IsNotNull(retrieved);

                Console.WriteLine($"[{retrieved.ThreadId}] -> {retrieved.Id}");
            }
        }

        [Test]
        public async Task Test_04_ModifyThreadRun()
        {
            var createAssistantRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(createAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var request = new CreateThreadRunRequest(assistant.Id);
            var run = await OpenAIClient.ThreadRunsEndpoint.CreateThreadRunAsync(thread.Id, request);

            // run in Queued and InProgress can't be modified
            var loopCounter = 0;
            while (run.Status == RunStatus.InProgress || run.Status == RunStatus.Queued)
            {
                await Task.Delay(2_000);
                loopCounter++;
                run = await OpenAIClient.ThreadRunsEndpoint.RetrieveRunAsync(thread.Id, run.Id);

                if (loopCounter == 10)
                {
                    Assert.Fail("Too much in long in InProgress/Queued status");
                }
            }

            var modified = await OpenAIClient.ThreadRunsEndpoint.ModifyThreadRunAsync(
                thread.Id,
                run.Id,
                new Dictionary<string, string>
                {
                    ["key"] = "value"
                });

            Assert.IsNotNull(modified);
            Assert.AreEqual(run.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("key", modified.Metadata.Keys.ToList());
            Assert.AreEqual("value", modified.Metadata["key"]);
        }

        [Test]
        public async Task Test_05_CancelRun()
        {
            var createAssistantRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(createAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var request = new CreateThreadRunRequest(assistant.Id);
            var run = await OpenAIClient.ThreadRunsEndpoint.CreateThreadRunAsync(thread.Id, request);

            run = await OpenAIClient.ThreadRunsEndpoint.CancelThreadRunAsync(thread.Id, run.Id);

            var loopCounter = 0;
            while (run.Status == RunStatus.Cancelling)
            {
                await Task.Delay(2_000);
                loopCounter++;
                run = await OpenAIClient.ThreadRunsEndpoint.RetrieveRunAsync(thread.Id, run.Id);

                if (loopCounter == 10)
                {
                    Assert.Fail("Too much in Cancelling status");
                }
            }

            Assert.AreEqual(RunStatus.Cancelled, run.Status);
        }
    }
}