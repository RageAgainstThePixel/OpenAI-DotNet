// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_02_Assistants : AbstractTestFixture
    {
        private AssistantResponse testAssistant;

        [Test]
        public async Task Test_01_CreateAssistant()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            const string testFilePath = "assistant_test_1.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, "assistants");
            File.Delete(testFilePath);
            Assert.IsFalse(File.Exists(testFilePath));
            var request = new CreateAssistantRequest(Model.GPT3_5_Turbo,
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
                files: new[] { file.Id });
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(request);
            Assert.IsNotNull(assistant);
            Assert.AreEqual("test-assistant", assistant.Name);
            Assert.AreEqual("Used for unit testing.", assistant.Description);
            Assert.AreEqual("You are test assistant", assistant.Instructions);
            Assert.AreEqual(Model.GPT3_5_Turbo.ToString(), assistant.Model);
            Assert.IsNotEmpty(assistant.Metadata);
            testAssistant = assistant;
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
                var retrieved = await OpenAIClient.AssistantsEndpoint.RetrieveAssistantAsync(assistant);
                Assert.IsNotNull(retrieved);
                Console.WriteLine($"{retrieved} -> {retrieved.CreatedAt}");
            }
        }

        [Test]
        public async Task Test_03_ModifyAssistants()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var request = new CreateAssistantRequest(
                model: Model.GPT4_Turbo,
                name: "Test modified",
                description: "Modified description",
                instructions: "You are modified test assistant");
            var assistant = await testAssistant.ModifyAsync(request);
            Assert.IsNotNull(assistant);
            Assert.AreEqual("Test modified", assistant.Name);
            Assert.AreEqual("Modified description", assistant.Description);
            Assert.AreEqual("You are modified test assistant", assistant.Instructions);
            Assert.AreEqual(Model.GPT4_Turbo.ToString(), assistant.Model);
            Assert.IsTrue(assistant.Metadata.ContainsKey("test"));
            Console.WriteLine($"{assistant.Id} -> modified");
        }

        [Test]
        public async Task Test_04_01_UploadAssistantFile()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            const string testFilePath = "assistant_test_2.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            var file = await testAssistant.UploadFileAsync(testFilePath);
            Assert.IsNotNull(file);
            Console.WriteLine($"uploaded -> {file.Id}");
        }

        [Test]
        public async Task Test_04_02_ListAssistantFiles()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var filesList = await testAssistant.ListFilesAsync();
            Assert.IsNotNull(filesList);
            Assert.IsNotEmpty(filesList.Items);
            Assert.IsTrue(filesList.Items.Count == 2);

            foreach (var file in filesList.Items)
            {
                Assert.IsNotNull(file);
                var retrieved = await testAssistant.RetrieveFileAsync(file);
                Assert.IsNotNull(retrieved);
                Assert.IsTrue(retrieved.Id == file.Id);
                Console.WriteLine($"{retrieved.AssistantId}'s file -> {retrieved.Id}");
                // TODO 400 Bad Request error when attempting to download assistant files. Likely OpenAI bug.
                //var downloadPath = await retrieved.DownloadFileAsync(Directory.GetCurrentDirectory(), true);
                //Console.WriteLine($"downloaded {retrieved} -> {downloadPath}");
                //Assert.IsTrue(File.Exists(downloadPath));
                //File.Delete(downloadPath);
                //Assert.IsFalse(File.Exists(downloadPath));
            }
        }

        [Test]
        public async Task Test_04_03_RemoveAssistantFile()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var filesList = await testAssistant.ListFilesAsync();
            Assert.IsNotNull(filesList);
            Assert.IsNotEmpty(filesList.Items);
            Assert.IsTrue(filesList.Items.Count == 2);
            var assistantFile = filesList.Items[0];
            Assert.IsNotNull(assistantFile);
            var isRemoved = await testAssistant.RemoveFileAsync(assistantFile);
            Assert.IsTrue(isRemoved);
            var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(assistantFile);
            Assert.IsTrue(isDeleted);
        }

        [Test]
        public async Task Test_04_04_DeleteAssistantFiles()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var filesList = await testAssistant.ListFilesAsync();
            Assert.IsNotNull(filesList);
            Assert.IsNotEmpty(filesList.Items);
            Assert.IsTrue(filesList.Items.Count == 1);
            var assistantFile = filesList.Items[0];
            Assert.IsNotNull(assistantFile);
            var isDeleted = await testAssistant.DeleteFileAsync(assistantFile);
            Assert.IsTrue(isDeleted);
        }

        [Test]
        public async Task Test_05_DeleteAssistant()
        {
            Assert.IsNotNull(testAssistant);
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var result = await testAssistant.DeleteAsync();
            Assert.IsTrue(result);
            Console.WriteLine($"{testAssistant.Id} -> deleted");
        }
    }
}