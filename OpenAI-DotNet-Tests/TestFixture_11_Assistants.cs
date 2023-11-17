using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_11_Assistants : AbstractTestFixture
    {
        private static string testAssistantId;

        [Test]
        public async Task Test_01_CreateAssistant()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            const string testFilePath = "assistant_test.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, "assistants");
            File.Delete(testFilePath);
            Assert.IsFalse(File.Exists(testFilePath));
            var request = new AssistantRequest("gpt-3.5-turbo-1106",
                name: "test-assistant",
                description: "Used for unit testing.",
                instructions: "You are test assistant",
                metadata: new Dictionary<string, string>
                {
                    ["int"] = "1",
                    ["test"] = Guid.NewGuid().ToString()
                },
                tools: new[]
                {
                    Tool.Retrieval
                },
                fileIds: new[] { file.Id });
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(request);
            Assert.IsNotNull(assistant);
            Assert.AreEqual("test-assistant", assistant.Name);
            Assert.AreEqual("Used for unit testing.", assistant.Description);
            Assert.AreEqual("You are test assistant", assistant.Instructions);
            Assert.AreEqual("gpt-3.5-turbo-1106", assistant.Model);
            Assert.IsNotEmpty(assistant.Metadata);
            testAssistantId = assistant.Id;
            Console.WriteLine($"{assistant} -> {assistant.Metadata["test"]}");
        }

        [Test]
        public async Task Test_02_ListAssistants()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();
            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Items);

            foreach (var assistant in assistantsList.Items)
            {
                Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
            }
        }

        [Test]
        public async Task Test_03_ListAssistantFiles()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testAssistantId));
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(testAssistantId);
            Assert.IsNotNull(filesList);
            Assert.IsNotEmpty(filesList.Items);

            foreach (var file in filesList.Items)
            {
                Assert.IsNotNull(file);
                var retrieved = await OpenAIClient.AssistantsEndpoint.RetrieveAssistantFileAsync(file.AssistantId, file.Id);
                Assert.IsNotNull(retrieved);
                Console.WriteLine($"{retrieved.AssistantId}'s file -> {retrieved.Id}");
            }
        }

        [Test]
        public async Task Test_04_DeleteAssistantFile()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testAssistantId));
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(testAssistantId);
            Assert.IsNotNull(filesList);
            Assert.IsNotEmpty(filesList.Items);

            foreach (var file in filesList.Items)
            {
                Assert.IsNotNull(file);
                var isRemoved = await OpenAIClient.AssistantsEndpoint.RemoveAssistantFileAsync(file);
                Assert.IsTrue(isRemoved);
                var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
                Assert.IsTrue(isDeleted);
            }

            filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(testAssistantId);
            Assert.IsNotNull(filesList);
            Assert.IsEmpty(filesList.Items);
        }

        [Test]
        public async Task Test_05_ModifyAssistants()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testAssistantId));
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var request = new AssistantRequest(
                model: "gpt-4-1106-preview",
                name: "Test modified",
                description: "Modified description",
                instructions: "You are modified test assistant");
            var assistant = await OpenAIClient.AssistantsEndpoint.ModifyAssistantAsync(testAssistantId, request);
            Assert.IsNotNull(assistant);
            Assert.AreEqual("Test modified", assistant.Name);
            Assert.AreEqual("Modified description", assistant.Description);
            Assert.AreEqual("You are modified test assistant", assistant.Instructions);
            Assert.AreEqual("gpt-4-1106-preview", assistant.Model);
            Assert.IsTrue(assistant.Metadata.ContainsKey("test"));
            Console.WriteLine($"{assistant.Id} -> modified");
        }

        [Test]
        public async Task Test_06_DeleteAssistant()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testAssistantId));
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var result = await OpenAIClient.AssistantsEndpoint.DeleteAssistantAsync(testAssistantId);
            Assert.IsTrue(result);
            Console.WriteLine($"{testAssistantId} -> deleted");
        }
    }
}