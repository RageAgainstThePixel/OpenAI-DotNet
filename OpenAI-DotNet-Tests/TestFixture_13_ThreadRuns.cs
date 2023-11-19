using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Threads;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    /// <summary>
    /// https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb
    /// </summary>
    internal class TestFixture_13_ThreadRuns : AbstractTestFixture
    {
        private static AssistantResponse testAssistant;
        private static ThreadResponse testThread;
        private static RunResponse testRun;

        [Test]
        public async Task Test_01_CreateRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new AssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: "gpt-4-1106-preview"));
            Assert.NotNull(assistant);
            testAssistant = assistant;
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
            Assert.NotNull(thread);

            try
            {
                var message = thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.NotNull(message);
                var run = await thread.CreateRunAsync(assistant);
                Assert.IsNotNull(run);
            }
            finally
            {
                await thread.DeleteAsync();
            }
        }

        [Test]
        public async Task Test_02_CreateThreadAndRun()
        {
            Assert.NotNull(testAssistant);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var run = await testAssistant.CreateThreadAndRunAsync();
            Assert.IsNotNull(run);
            testRun = run;
            var thread = await run.GetThreadAsync();
            Assert.NotNull(thread);
            testThread = thread;
        }

        [Test]
        public async Task Test_04_ModifyRun()
        {
            Assert.NotNull(testRun);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            // run in Queued and InProgress can't be modified
            var run = await testRun.WaitForStatusAsync();
            Assert.IsNotNull(run);
            Assert.IsTrue(run.Status == RunStatus.Completed);
            var metadata = new Dictionary<string, string>
            {
                ["test"] = nameof(Test_04_ModifyRun)
            };
            var modified = await run.ModifyAsync(metadata);
            Assert.IsNotNull(modified);
            Assert.AreEqual(run.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("test", modified.Metadata.Keys.ToList());
            Assert.AreEqual(nameof(Test_04_ModifyRun), modified.Metadata["test"]);
        }

        [Test]
        public async Task Test_03_ListRuns()
        {
            Assert.NotNull(testThread);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var list = await testThread.ListRunsAsync();
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var run in list.Items)
            {
                var retrievedRun = await run.UpdateAsync();
                Assert.IsNotNull(retrievedRun);
            }
        }

        [Test]
        public async Task Test_05_CancelRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var run = await testThread.CreateRunAsync(testAssistant);
            Assert.IsNotNull(run);
            run = await run.CancelAsync();
            Assert.IsNotNull(run);
            Assert.IsTrue(run.Status == RunStatus.Cancelling);
            // waiting while run is cancelling
            run = await run.WaitForStatusChangeAsync();
            Assert.IsTrue(run.Status == RunStatus.Cancelled);
        }

        //[Test]
        //public async Task Test_06_SubmitToolOutput()
        //{
        //    var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
        //    var createThreadRequest = new CreateThreadRequest(new List<Message>
        //    {
        //        "I'm in Kuala-Lumpur, please tell me what's the temperature in celsius now?"
        //    });

        //    var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(createThreadRequest);

        //    var function = new Function(
        //        nameof(WeatherService.GetCurrentWeather),
        //        "Get the current weather in a given location",
        //        new JsonObject
        //        {
        //            ["type"] = "object",
        //            ["properties"] = new JsonObject
        //            {
        //                ["location"] = new JsonObject
        //                {
        //                    ["type"] = "string",
        //                    ["description"] = "The city and state, e.g. San Francisco, CA"
        //                },
        //                ["unit"] = new JsonObject
        //                {
        //                    ["type"] = "string",
        //                    ["enum"] = new JsonArray { "celsius", "fahrenheit" }
        //                }
        //            },
        //            ["required"] = new JsonArray { "location", "unit" }
        //        });

        //    var request = new CreateRunRequest(assistant, tools: new Tool[] { function });
        //    var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

        //    Assert.AreEqual(RunStatus.RequiresAction, run.Status);
        //    Assert.IsNotNull(run.RequiredAction);
        //    Assert.AreEqual("submit_tool_outputs", run.RequiredAction.Type);
        //    Assert.IsNotNull(run.RequiredAction.SubmitToolOutputs);
        //    Assert.IsNotEmpty(run.RequiredAction.SubmitToolOutputs.ToolCalls);
        //    Assert.AreEqual(1, run.RequiredAction.SubmitToolOutputs.ToolCalls.Count);
        //    var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
        //    Assert.AreEqual("function", toolCall.Type);
        //    Assert.IsNotNull(toolCall.FunctionCall);
        //    Assert.AreEqual(nameof(WeatherService.GetCurrentWeather), toolCall.FunctionCall.Name);
        //    Assert.IsNotNull(toolCall.FunctionCall.Arguments);

        //    var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(toolCall.FunctionCall.Arguments);
        //    var functionResult = WeatherService.GetCurrentWeather(functionArgs);
        //    var submitRequest = new SubmitToolOutputsRequest(new List<ToolOutput>
        //    {
        //        new ToolOutput(toolCall.Id, functionResult)
        //    });

        //    run = await OpenAIClient.ThreadsEndpoint.SubmitToolOutputsAsync(thread.Id, run.Id, submitRequest);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);
        //    Assert.AreEqual(RunStatus.Completed, run.Status);
        //}

        //[Test]
        //public async Task Test_07_ListRunSteps()
        //{
        //    var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
        //    var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
        //    var request = new CreateRunRequest(assistant);
        //    var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

        //    var list = await OpenAIClient.ThreadsEndpoint.ListRunStepsAsync(thread.Id, run.Id);

        //    Assert.IsNotNull(list);
        //    Assert.IsNotEmpty(list.Items);

        //    foreach (var step in list.Items)
        //    {
        //        var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveRunStepAsync(thread.Id, run.Id, step.Id);

        //        Assert.IsNotNull(retrieved);

        //        Console.WriteLine($"[{retrieved.ThreadId}] -> {retrieved.Id}");
        //    }
        //}
    }
}