// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Models;
using OpenAI.Tests.Weather;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    /// <summary>
    /// https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb
    /// </summary>
    internal class TestFixture_03_Threads : AbstractTestFixture
    {
        private static RunResponse testRun;
        private static ThreadResponse testThread;
        private static MessageResponse testMessage;
        private static AssistantResponse testAssistant;

        [Test]
        public async Task Test_01_CreateThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest(
                new List<Message>
                {
                   "Test message"
                },
                new Dictionary<string, string>
                {
                    ["test"] = nameof(Test_01_CreateThread)
                }));
            Assert.IsNotNull(thread);
            Assert.IsNotNull(thread.Metadata);
            Assert.IsNotEmpty(thread.Metadata);
            testThread = thread;
            Console.WriteLine($"Create thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_02_RetrieveThread()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await testThread.UpdateAsync();
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThread.Id, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_03_ModifyThread()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var newMetadata = new Dictionary<string, string>
            {
                ["test"] = nameof(Test_03_ModifyThread)
            };
            var thread = await testThread.ModifyAsync(newMetadata);
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThread.Id, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Console.WriteLine($"Modify thread {thread.Id} -> {thread.Metadata["test"]}");
        }

        [Test]
        public async Task Test_04_01_CreateMessage()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            const string testFilePath = "assistant_test_1.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, "assistants");
            Assert.NotNull(file);
            File.Delete(testFilePath);
            Assert.IsFalse(File.Exists(testFilePath));
            await testThread.CreateMessageAsync("hello world!");
            var request = new CreateMessageRequest("Test create message",
                new[] { file.Id },
                new Dictionary<string, string>
                {
                    ["test"] = nameof(Test_04_01_CreateMessage)
                });
            MessageResponse message;
            try
            {
                message = await testThread.CreateMessageAsync(request);
            }
            finally
            {
                await CleanupFileAsync(file);
            }

            Assert.IsNotNull(message);
            Assert.AreEqual(testThread.Id, message.ThreadId);
            testMessage = message;
        }

        [Test]
        public async Task Test_04_02_ListMessages()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var message1 = await testThread.CreateMessageAsync("Test message 1");
            Assert.IsNotNull(message1);
            var message2 = await testThread.CreateMessageAsync("Test message 2");
            Assert.IsNotNull(message2);
            var list = await testThread.ListMessagesAsync();
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var message in list.Items)
            {
                Assert.NotNull(message);
                var threadMessage = await testThread.RetrieveMessageAsync(message);
                Assert.NotNull(threadMessage);
                Console.WriteLine($"[{threadMessage.Id}] {threadMessage.Role}: {threadMessage.PrintContent()}");
                var updated = await message.UpdateAsync();
                Assert.IsNotNull(updated);
            }
        }

        [Test]
        public async Task Test_04_03_ModifyMessage()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(testMessage);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var metadata = new Dictionary<string, string>
            {
                ["test"] = nameof(Test_04_03_ModifyMessage)
            };
            var modified = await testMessage.ModifyAsync(metadata);
            Assert.IsNotNull(modified);
            Assert.IsNotNull(modified.Metadata);
            Assert.IsTrue(modified.Metadata["test"].Equals(nameof(Test_04_03_ModifyMessage)));
            Console.WriteLine($"Modify message metadata: {modified.Id} -> {modified.Metadata["test"]}");
            metadata.Add("test2", nameof(Test_04_03_ModifyMessage));
            var modifiedThreadMessage = await testThread.ModifyMessageAsync(modified, metadata);
            Assert.IsNotNull(modifiedThreadMessage);
            Assert.IsNotNull(modifiedThreadMessage.Metadata);
            Console.WriteLine($"Modify message metadata: {modifiedThreadMessage.Id} -> {string.Join("\n", modifiedThreadMessage.Metadata.Select(meta => $"[{meta.Key}] {meta.Value}"))}");
        }

        [Test]
        public async Task Test_04_04_UploadAndDownloadMessageFiles()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var file1 = await CreateTestFileAsync("test_1.txt");
            var file2 = await CreateTestFileAsync("test_2.txt");
            try
            {
                var createRequest = new CreateMessageRequest("Test content with files", new[] { file1.Id, file2.Id });
                var message = await testThread.CreateMessageAsync(createRequest);
                var fileList = await message.ListFilesAsync();
                Assert.IsNotNull(fileList);
                Assert.AreEqual(2, fileList.Items.Count);

                foreach (var file in fileList.Items)
                {
                    var retrieved = await message.RetrieveFileAsync(file);
                    Assert.IsNotNull(retrieved);
                    Console.WriteLine(file.Id);
                    // TODO 400 bad request errors. Likely OpenAI bug downloading message file content.
                    //var filePath = await message.DownloadFileContentAsync(file, Directory.GetCurrentDirectory(), true);
                    //Assert.IsFalse(string.IsNullOrWhiteSpace(filePath));
                    //Assert.IsTrue(File.Exists(filePath));
                    //File.Delete(filePath);
                }

                var threadList = await testThread.ListFilesAsync(message);
                Assert.IsNotNull(threadList);
                Assert.IsNotEmpty(threadList.Items);

                //foreach (var file in threadList.Items)
                //{
                //    // TODO 400 bad request errors. Likely OpenAI bug downloading message file content.
                //    var filePath = await file.DownloadContentAsync(Directory.GetCurrentDirectory(), true);
                //    Assert.IsFalse(string.IsNullOrWhiteSpace(filePath));
                //    Assert.IsTrue(File.Exists(filePath));
                //    File.Delete(filePath);
                //}
            }
            finally
            {
                await CleanupFileAsync(file1);
                await CleanupFileAsync(file2);
            }
        }

        [Test]
        public async Task Test_05_DeleteThread()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var isDeleted = await testThread.DeleteAsync();
            Assert.IsTrue(isDeleted);
            Console.WriteLine($"Deleted thread {testThread.Id}");
        }

        [Test]
        public async Task Test_06_01_CreateRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: Model.GPT4_Turbo));
            Assert.NotNull(assistant);
            testAssistant = assistant;
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
            Assert.NotNull(thread);

            try
            {
                var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.NotNull(message);
                var run = await thread.CreateRunAsync(assistant);
                Assert.IsNotNull(run);
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
            }
            finally
            {
                await thread.DeleteAsync();
            }
        }

        [Test]
        public async Task Test_06_02_CreateThreadAndRun()
        {
            Assert.NotNull(testAssistant);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var messages = new List<Message> { "I need to solve the equation `3x + 11 = 14`. Can you help me?" };
            var threadRequest = new CreateThreadRequest(messages);
            var run = await testAssistant.CreateThreadAndRunAsync(threadRequest);
            Assert.IsNotNull(run);
            Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
            testRun = run;
            var thread = await run.GetThreadAsync();
            Assert.NotNull(thread);
            testThread = thread;
        }

        [Test]
        public async Task Test_06_03_ListRunsAndSteps()
        {
            Assert.NotNull(testThread);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var runList = await testThread.ListRunsAsync();
            Assert.IsNotNull(runList);
            Assert.IsNotEmpty(runList.Items);

            foreach (var run in runList.Items)
            {
                Assert.IsNotNull(run);
                Assert.IsNotNull(run.Client);
                var retrievedRun = await run.UpdateAsync();
                Assert.IsNotNull(retrievedRun);
                var threadRun = await testThread.RetrieveRunAsync(run.Id);
                Assert.IsNotNull(threadRun);
                Assert.IsTrue(retrievedRun.Id == threadRun.Id);
                Console.WriteLine($"[{retrievedRun.Id}] {retrievedRun.Status} | {retrievedRun.CreatedAt}");
            }
        }

        [Test]
        public async Task Test_06_04_ModifyRun()
        {
            Assert.NotNull(testRun);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            // run in Queued and InProgress can't be modified
            var run = await testRun.WaitForStatusChangeAsync();
            Assert.IsNotNull(run);
            Assert.IsTrue(run.Status == RunStatus.Completed);
            var metadata = new Dictionary<string, string>
            {
                ["test"] = nameof(Test_06_04_ModifyRun)
            };
            var modified = await run.ModifyAsync(metadata);
            Assert.IsNotNull(modified);
            Assert.AreEqual(run.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("test", modified.Metadata.Keys.ToList());
            Assert.AreEqual(nameof(Test_06_04_ModifyRun), modified.Metadata["test"]);
        }

        [Test]
        public async Task Test_06_05_CancelRun()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(testAssistant);
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var run = await testThread.CreateRunAsync(testAssistant);
            Assert.IsNotNull(run);
            Assert.IsTrue(run.Status == RunStatus.Queued);
            run = await run.CancelAsync();
            Assert.IsNotNull(run);
            Assert.IsTrue(run.Status == RunStatus.Cancelling);

            try
            {
                // waiting while run is cancelling
                run = await run.WaitForStatusChangeAsync();
            }
            catch (Exception e)
            {
                // Sometimes runs will get stuck in Cancelling state,
                // for now we just log when it happens.
                Console.WriteLine(e);
            }

            Assert.IsTrue(run.Status is RunStatus.Cancelled or RunStatus.Cancelling);
        }

        [Test]
        public async Task Test_06_06_TestCleanup()
        {
            if (testAssistant != null)
            {
                var isDeleted = await testAssistant.DeleteAsync();
                Assert.IsTrue(isDeleted);
            }

            if (testThread != null)
            {
                var isDeleted = await testThread.DeleteAsync();
                Assert.IsTrue(isDeleted);
            }
        }

        [Test]
        public async Task Test_07_01_SubmitToolOutput()
        {
            var tools = new List<Tool>
            {
                Tool.CodeInterpreter,
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
            };
            var assistantRequest = new CreateAssistantRequest(tools: tools, instructions: "You are a helpful weather assistant. Use the appropriate unit based on geographical location.");
            testAssistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
            var run = await testAssistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature now?");
            testThread = await run.GetThreadAsync();
            // waiting while run is Queued and InProgress
            run = await run.WaitForStatusChangeAsync();
            Assert.IsNotNull(run);
            Assert.AreEqual(RunStatus.RequiresAction, run.Status);
            Assert.IsNotNull(run.RequiredAction);
            Assert.IsNotNull(run.RequiredAction.SubmitToolOutputs);
            Assert.IsNotEmpty(run.RequiredAction.SubmitToolOutputs.ToolCalls);

            var runStepList = await run.ListRunStepsAsync();
            Assert.IsNotNull(runStepList);
            Assert.IsNotEmpty(runStepList.Items);

            foreach (var runStep in runStepList.Items)
            {
                Assert.IsNotNull(runStep);
                Assert.IsNotNull(runStep.Client);
                var retrievedRunStep = await runStep.UpdateAsync();
                Assert.IsNotNull(retrievedRunStep);
                Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiresAt}");
                var retrieveStepRunStep = await run.RetrieveRunStepAsync(runStep.Id);
                Assert.IsNotNull(retrieveStepRunStep);
            }

            var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
            Assert.IsTrue(run.RequiredAction.SubmitToolOutputs.ToolCalls.Count == 1);
            Assert.AreEqual("function", toolCall.Type);
            Assert.IsNotNull(toolCall.FunctionCall);
            Assert.IsTrue(toolCall.FunctionCall.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Assert.IsNotNull(toolCall.FunctionCall.Arguments);
            Console.WriteLine($"tool call arguments: {toolCall.FunctionCall.Arguments}");

            // Invoke all the tool call functions and return the tool outputs.
            var toolOutputs = await testAssistant.GetToolOutputsAsync(run.RequiredAction.SubmitToolOutputs.ToolCalls);

            foreach (var toolOutput in toolOutputs)
            {
                Console.WriteLine($"tool output: {toolOutput}");
            }

            run = await run.SubmitToolOutputsAsync(toolOutputs);
            // waiting while run in Queued and InProgress
            run = await run.WaitForStatusChangeAsync();
            Assert.AreEqual(RunStatus.Completed, run.Status);
            runStepList = await run.ListRunStepsAsync();

            foreach (var runStep in runStepList.Items)
            {
                Assert.IsNotNull(runStep);
                Assert.IsNotNull(runStep.Client);
                var retrievedRunStep = await runStep.UpdateAsync();
                Assert.IsNotNull(retrievedRunStep);
                Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {(runStep.ExpiresAtUnixTimeSeconds.HasValue ? runStep.ExpiresAt : runStep.CompletedAt)}");
                if (runStep.StepDetails.ToolCalls == null) { continue; }

                foreach (var runStepToolCall in runStep.StepDetails.ToolCalls)
                {
                    Console.WriteLine($"[{runStep.Id}][{runStepToolCall.Type}][{runStepToolCall.Id}] {runStepToolCall.FunctionCall.Name}: {runStepToolCall.FunctionCall.Output}");
                }
            }

            var messages = await run.ListMessagesAsync();
            Assert.IsNotNull(messages);
            Assert.IsNotEmpty(messages.Items);

            foreach (var message in messages.Items.OrderBy(response => response.CreatedAt))
            {
                Assert.IsNotNull(message);
                Assert.IsNotEmpty(message.Content);
                Console.WriteLine($"{message.Role}: {message.PrintContent()}");
            }
        }

        [Test]
        public async Task Test_07_02_TestCleanup()
        {
            if (testAssistant != null)
            {
                var isDeleted = await testAssistant.DeleteAsync();
                Assert.IsTrue(isDeleted);
            }

            if (testThread != null)
            {
                var isDeleted = await testThread.DeleteAsync();
                Assert.IsTrue(isDeleted);
            }
        }

        private async Task<FileResponse> CreateTestFileAsync(string filePath)
        {
            await File.WriteAllTextAsync(filePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(filePath));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(filePath, "assistants");
            File.Delete(filePath);
            Assert.IsFalse(File.Exists(filePath));
            return file;
        }

        private async Task CleanupFileAsync(FileResponse file)
        {
            var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
            Assert.IsTrue(isDeleted);
        }
    }
}