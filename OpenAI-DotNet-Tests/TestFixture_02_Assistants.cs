// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_02_Assistants : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Assistants()
        {
            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);
            const string testFilePath = "assistant_test_1.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            FileResponse file = null;

            try
            {
                try
                {
                    file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, FilePurpose.Assistants);
                }
                finally
                {
                    if (File.Exists(testFilePath))
                    {
                        File.Delete(testFilePath);
                    }

                    Assert.IsFalse(File.Exists(testFilePath));
                }

                var request = new CreateAssistantRequest(Model.GPT4_Turbo,
                    name: "test-assistant",
                    description: "Used for unit testing.",
                    instructions: "You are test assistant",
                    toolResources: new FileSearchResources(new List<string> { file.Id }),
                    metadata: new Dictionary<string, string>
                    {
                        ["int"] = "1",
                        ["test"] = Guid.NewGuid().ToString()
                    },
                    tools: new[] { new Tool(new FileSearchOptions(15, new RankingOptions("auto", 0.5f))) });
                var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(request);
                Assert.IsNotNull(assistant);

                try
                {
                    Assert.AreEqual("test-assistant", assistant.Name);
                    Assert.AreEqual("Used for unit testing.", assistant.Description);
                    Assert.AreEqual("You are test assistant", assistant.Instructions);
                    Assert.AreEqual(Model.GPT4_Turbo.ToString(), assistant.Model);
                    Assert.IsNotEmpty(assistant.Metadata);
                    Console.WriteLine($"{assistant} -> {assistant.Metadata["test"]}");

                    var modifiedAssistant = await assistant.ModifyAsync(new(
                        model: Model.GPT4o,
                        name: "Test modified",
                        description: "Modified description",
                        instructions: "You are modified test assistant",
                        metadata: new Dictionary<string, string>
                        {
                            ["int"] = "2",
                            ["test"] = assistant.Metadata["test"]
                        }));
                    Assert.IsNotNull(modifiedAssistant);
                    Assert.AreEqual("Test modified", modifiedAssistant.Name);
                    Assert.AreEqual("Modified description", modifiedAssistant.Description);
                    Assert.AreEqual("You are modified test assistant", modifiedAssistant.Instructions);
                    Assert.AreEqual(Model.GPT4o.ToString(), modifiedAssistant.Model);
                    Assert.IsTrue(modifiedAssistant.Metadata.ContainsKey("test"));
                    Assert.AreEqual("2", modifiedAssistant.Metadata["int"]);
                    Assert.AreEqual(modifiedAssistant.Metadata["test"], assistant.Metadata["test"]);
                    Console.WriteLine($"modified assistant -> {modifiedAssistant.Id}");

                    var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();
                    Assert.IsNotNull(assistantsList);
                    Assert.IsNotEmpty(assistantsList.Items);

                    foreach (var asst in assistantsList.Items)
                    {
                        var retrievedAsst = await OpenAIClient.AssistantsEndpoint.RetrieveAssistantAsync(asst);
                        Assert.IsNotNull(retrievedAsst);

                        var updatedAsst = await retrievedAsst.UpdateAsync();
                        Assert.IsNotNull(updatedAsst);
                    }
                }
                finally
                {
                    var isDeleted = await assistant.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
            finally
            {
                if (file != null)
                {
                    var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
                    Assert.IsTrue(isDeleted);
                }
            }
        }
    }
}
