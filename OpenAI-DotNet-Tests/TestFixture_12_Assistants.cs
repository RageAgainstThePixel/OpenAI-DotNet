using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenAI.Assistants;

namespace OpenAI.Tests
{
    internal class TestFixture_12_Assistants : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_CreateAssistant()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);

            var testData = "Some useful knowledge";
            var fileName = "test.txt";
            await File.WriteAllTextAsync(fileName, testData);
            Assert.IsTrue(File.Exists(fileName));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(fileName, "assistants");

            var assistantFileId = file.Id;

            var request = new CreateAssistantRequest("gpt-3.5-turbo-1106")
            {
                Name = "Test",
                Description = "Test description",
                Instructions = "You are test assistant",
                Metadata = new Dictionary<string, object>
                {
                    ["int"] = "1",
                    ["text"] = "test"
                },
                Tools = new List<AssistantTool>
                {
                    new(AssistantToolType.Retrieval)
                },
                FileIds = new List<string> { assistantFileId }
            };

            var result = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual("Test description", result.Description);
            Assert.AreEqual("You are test assistant", result.Instructions);
            Assert.AreEqual("gpt-3.5-turbo-1106", result.Model);
        }

        [Test]
        public async Task Test_02_ListAssistants()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);

            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Data);

            foreach (var assistant in assistantsList.Data)
            {
                var retrieved = await OpenAIClient.AssistantsEndpoint.RetrieveAssistantAsync(assistant.Id);
                Assert.NotNull(retrieved);

                Console.WriteLine($"[{retrieved.Id}] {retrieved.Name}: {retrieved.Description}");
            }
        }

        [Test]
        public async Task Test_03_ListAssistantFiles()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Data);

            foreach (var assistant in assistantsList.Data)
            {
                var filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(assistant.Id);

                Assert.IsNotNull(assistantsList);
                Assert.IsNotEmpty(assistantsList.Data);

                foreach (var file in filesList.Data)
                {
                    Assert.IsNotNull(file);

                    var retrieved =
                        await OpenAIClient.AssistantsEndpoint.RetrieveAssistantFileAsync(file.AssistantId, file.Id);

                    Assert.IsNotNull(retrieved);
                    
                    Console.WriteLine($"{retrieved.AssistantId}'s file -> {retrieved.Id}");
                }
            }
        }

        [Test]
        public async Task Test_04_DeleteAssistantFile()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Data);

            foreach (var assistant in assistantsList.Data)
            {
                var filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(assistant.Id);

                Assert.IsNotNull(filesList);
                Assert.IsNotEmpty(filesList.Data);

                foreach (var file in filesList.Data)
                {
                    Assert.IsNotNull(file);

                    var isDeleted =
                        await OpenAIClient.AssistantsEndpoint.DeleteAssistantFileAsync(file.AssistantId, file.Id);

                    Assert.IsTrue(isDeleted);
                }
                
                filesList = await OpenAIClient.AssistantsEndpoint.ListAssistantFilesAsync(assistant.Id);
                
                Assert.IsNotNull(filesList);
                Assert.IsEmpty(filesList.Data);
            }
        }
        
        [Test]
        public async Task Test_05_ModifyAssistants()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Data);

            foreach (var assistant in assistantsList.Data)
            {
                var request = new ModifyAssistantRequest
                {
                    Name = "Test modified",
                    Description = "Modified description",
                    Instructions = "You are modified test assistant",
                    Model = "gpt-3.5-turbo",
                    Tools = new List<AssistantTool>()
                };

                var modified = await OpenAIClient.AssistantsEndpoint.ModifyAssistantAsync(assistant.Id, request);

                Assert.AreEqual("Test modified", modified.Name);
                Assert.AreEqual("Modified description", modified.Description);
                Assert.AreEqual("You are modified test assistant", modified.Instructions);
                Assert.AreEqual("gpt-3.5-turbo", modified.Model);

                Console.WriteLine($"{assistant.Id} -> modified");
            }
        }

        [Test]
        public async Task Test_06_DeleteAssistant()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

            Assert.IsNotNull(assistantsList);
            Assert.IsNotEmpty(assistantsList.Data);

            foreach (var assistant in assistantsList.Data)
            {
                var result = await OpenAIClient.AssistantsEndpoint.DeleteAssistantAsync(assistant.Id);
                Assert.IsTrue(result);
                Console.WriteLine($"{assistant.Id} -> deleted");
            }

            assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();
            Assert.IsNotNull(assistantsList);
            Assert.IsEmpty(assistantsList.Data);
        }
    }
}