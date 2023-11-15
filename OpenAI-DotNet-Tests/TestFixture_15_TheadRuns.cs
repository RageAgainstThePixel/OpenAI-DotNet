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
    }
}