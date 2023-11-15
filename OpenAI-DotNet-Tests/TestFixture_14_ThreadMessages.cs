using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.ThreadMessages;
using OpenAI.Threads;
using Message = OpenAI.Threads.Message;

namespace OpenAI.Tests
{
    internal class TestFixture_14_TheadMessages : AbstractTestFixture
    {
        private static CreateThreadRequest TestThread { get; } = new CreateThreadRequest(
            new List<Message>
            {
                new Message("Test message")
            },
            new Dictionary<string, string>
            {
                ["text"] = "test"
            }
        );

        [Test]
        public async Task Test_01_CreateThreadMessage()
        {
            Assert.IsNotNull(OpenAIClient.ThreadMessagesEndpoint);

            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            var file = await CreateFileForAssistant();

            var request = new CreateThreadMessageRequest("Test content")
            {
                FileIds = new[] { file.Id },
                Metadata = new Dictionary<string, string>
                {
                    ["test"] = "value"
                }
            };

            var created = await OpenAIClient.ThreadMessagesEndpoint.CreateThreadMessageAsync(thread.Id, request);

            Assert.IsNotNull(created);
            Assert.AreEqual("thread.message", created.Object);
            Assert.AreEqual(ThreadRole.User, created.Role);
            Assert.AreEqual(thread.Id, created.ThreadId);

            Assert.IsNotNull(created.Content);
            Assert.AreEqual(1, created.Content.Length);
            Assert.AreEqual(ContentType.Text, created.Content[0].Type);
            Assert.AreEqual("Test content", created.Content[0].Text.Value);

            Assert.IsNotEmpty(created.FileIds);
            Assert.AreEqual(1, created.FileIds.Length);
            Assert.AreEqual(file.Id, created.FileIds[0]);

            Assert.IsNotNull(created.Metadata);
            Assert.Contains("test", created.Metadata.Keys);
            Assert.AreEqual("value", created.Metadata["test"]);
        }

        [Test]
        public async Task Test_02_ListThreadMessages()
        {
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            
            var message1 = await OpenAIClient.ThreadMessagesEndpoint.CreateThreadMessageAsync(
                thread.Id, new CreateThreadMessageRequest("Test content"));
            
            var message2 = await OpenAIClient.ThreadMessagesEndpoint.CreateThreadMessageAsync(
                thread.Id, new CreateThreadMessageRequest("Test content 2"));

            var list = await OpenAIClient.ThreadMessagesEndpoint.ListThreadMessagesAsync(thread.Id);

            Assert.IsNotNull(list);

            foreach (var message in list.Data)
            {
                var retrieved = await OpenAIClient.ThreadMessagesEndpoint.RetrieveThreadMessageAsync(message.ThreadId, message.Id);
                Assert.NotNull(retrieved);

                Console.WriteLine($"[{retrieved.Id}] {retrieved.Content}");
            }
        }

        [Test]
        public async Task Test_03_ModifyThreadMessage()
        {
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            
            var message = await OpenAIClient.ThreadMessagesEndpoint.CreateThreadMessageAsync(
                thread.Id, new CreateThreadMessageRequest("Test content"));

            var modified = await OpenAIClient.ThreadMessagesEndpoint.ModifyThreadMessageAsync(
                message.ThreadId,
                message.Id,
                new Dictionary<string, string>
                {
                    ["test"] = "value"
                });
            
            Assert.IsNotNull(modified);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("test", modified.Metadata.Keys);
            Assert.AreEqual("value", modified.Metadata["test"]);
        }

        [Test]
        public async Task Test_04_ListMessageFiles()
        {
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            var file1 = await CreateFileForAssistant();
            var file2 = await CreateFileForAssistant();

            var createRequest = new CreateThreadMessageRequest("Test content")
            {
                FileIds = new[] { file1.Id, file2.Id }
            };

            var message = await OpenAIClient.ThreadMessagesEndpoint.CreateThreadMessageAsync(thread.Id, createRequest);

            var list = await OpenAIClient.ThreadMessagesEndpoint.ListMessageFilesAsync(message.ThreadId, message.Id);
            
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Data.Count);

            foreach (var file in list.Data)
            {
                var retrieved =
                    await OpenAIClient.ThreadMessagesEndpoint.RetrieveMessageFile(thread.Id, message.Id, file.Id);

                Assert.IsNotNull(retrieved);

                Console.WriteLine($"[{retrieved.MessageId}] -> {retrieved.Id}");
            }
        }

        private async Task<FileData> CreateFileForAssistant()
        {
            var testData = "Some useful knowledge";
            var fileName = "test.txt";
            await File.WriteAllTextAsync(fileName, testData);
            Assert.IsTrue(File.Exists(fileName));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(fileName, "assistants");
            return file;
        }
    }
}