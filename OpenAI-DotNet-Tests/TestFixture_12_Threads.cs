using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_12_Threads : AbstractTestFixture
    {
        private static string testThreadId;

        [Test]
        public async Task Test_01_CreateThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest(
                new List<Message>
                {
                    new Message("Test message")
                },
                new Dictionary<string, string>
                {
                    ["text"] = "test"
                }));
            Assert.IsNotNull(thread);
            Assert.IsNotNull(thread.Metadata);
            Assert.IsNotEmpty(thread.Metadata);
            Assert.Contains("text", thread.Metadata.Keys.ToList());
            Assert.AreEqual("test", thread.Metadata["text"]);
            testThreadId = thread.Id;
            Console.WriteLine($"Created thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_02_RetrieveThread()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.RetrieveThreadAsync(testThreadId);
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThreadId, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Assert.Contains("text", thread.Metadata.Keys.ToList());
            Assert.AreEqual("test", thread.Metadata["text"]);
            Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_03_ModifyThread()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var newMetadata = new Dictionary<string, string>
            {
                ["text"] = "test2"
            };
            var thread = await OpenAIClient.ThreadsEndpoint.ModifyThreadAsync(testThreadId, newMetadata);
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThreadId, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Assert.Contains("text", thread.Metadata.Keys.ToList());
            Assert.AreEqual("test2", thread.Metadata["text"]);
            Console.WriteLine($"Modified thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_04_01_CreateMessage()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var file = await CreateFileForAssistant();

            var request = new CreateMessageRequest("Test content",
                new[] { file.Id },
                new Dictionary<string, string>
                {
                    ["test"] = "value"
                });

            var message = await OpenAIClient.ThreadsEndpoint.CreateThreadMessageAsync(testThreadId, request);

            Assert.IsNotNull(message);
            Assert.AreEqual(Role.User, message.Role);
            Assert.AreEqual(testThreadId, message.ThreadId);

            Assert.IsNotNull(message.Content);
            Assert.AreEqual(1, message.Content.Count);
            Assert.AreEqual(ContentType.Text, message.Content[0].Type);
            Assert.AreEqual("Test content", message.Content[0].Text.Value);

            Assert.IsNotEmpty(message.FileIds);
            Assert.AreEqual(1, message.FileIds.Count);
            Assert.AreEqual(file.Id, message.FileIds[0]);

            Assert.IsNotNull(message.Metadata);
            Assert.Contains("test", message.Metadata.Keys.ToList());
            Assert.AreEqual("value", message.Metadata["test"]);
        }

        [Test]
        public async Task Test_04_02_ListMessages()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var message1 = await OpenAIClient.ThreadsEndpoint.CreateThreadMessageAsync(testThreadId, new CreateMessageRequest("Test content"));
            Assert.IsNotNull(message1);
            var message2 = await OpenAIClient.ThreadsEndpoint.CreateThreadMessageAsync(testThreadId, new CreateMessageRequest("Test content 2"));
            Assert.IsNotNull(message2);
            var list = await OpenAIClient.ThreadsEndpoint.ListThreadMessagesAsync(testThreadId);
            Assert.IsNotNull(list);

            foreach (var message in list.Items)
            {
                var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveThreadMessageAsync(testThreadId, message.Id);
                Assert.NotNull(retrieved);
                Console.WriteLine($"[{retrieved.Id}] {retrieved.Content}");
            }
        }

        [Test]
        public async Task Test_04_03_ModifyMessage()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var message = await OpenAIClient.ThreadsEndpoint.CreateThreadMessageAsync(testThreadId, new CreateMessageRequest("Test content"));
            var modified = await OpenAIClient.ThreadsEndpoint.ModifyThreadMessageAsync(
                message.ThreadId,
                message.Id,
                new Dictionary<string, string>
                {
                    ["test"] = "value"
                });

            Assert.IsNotNull(modified);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("test", modified.Metadata.Keys.ToList());
            Assert.AreEqual("value", modified.Metadata["test"]);
        }

        [Test]
        public async Task Test_04_04_ListMessageFiles()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var file1 = await CreateFileForAssistant();
            var file2 = await CreateFileForAssistant();
            var createRequest = new CreateMessageRequest("Test content", new[] { file1.Id, file2.Id });
            var message = await OpenAIClient.ThreadsEndpoint.CreateThreadMessageAsync(testThreadId, createRequest);
            var list = await OpenAIClient.ThreadsEndpoint.ListMessageFilesAsync(message.ThreadId, message.Id);

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Items.Count);

            foreach (var file in list.Items)
            {
                var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveMessageFileAsync(testThreadId, message.Id, file.Id);

                Assert.IsNotNull(retrieved);

                Console.WriteLine($"[{retrieved.MessageId}] -> {retrieved.Id}");
            }
        }

        [Test]
        public async Task Test_05_DeleteThread()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var isDeleted = await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(testThreadId);
            Assert.IsTrue(isDeleted);
            Console.WriteLine($"Deleted thread {testThreadId}");
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