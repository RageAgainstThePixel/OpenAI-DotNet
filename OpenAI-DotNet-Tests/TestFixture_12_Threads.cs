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
        private static string testMessageId;

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
                    ["test"] = "01"
                }));
            Assert.IsNotNull(thread);
            Assert.IsNotNull(thread.Metadata);
            Assert.IsNotEmpty(thread.Metadata);
            testThreadId = thread.Id;
            Console.WriteLine($"Create thread {thread.Id} -> {thread.CreatedAt}");
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
            Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_03_ModifyThread()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var newMetadata = new Dictionary<string, string>
            {
                ["test"] = "03"
            };
            var thread = await OpenAIClient.ThreadsEndpoint.ModifyThreadAsync(testThreadId, newMetadata);
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThreadId, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Console.WriteLine($"Modify thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_04_01_CreateMessage()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var file = await CreateTestFileAsync("test.txt");
            var request = new CreateMessageRequest("Test create message",
                new[] { file.Id },
                new Dictionary<string, string>
                {
                    ["test"] = "04_01"
                });
            MessageResponse message;

            try
            {
                message = await OpenAIClient.ThreadsEndpoint.CreateMessageAsync(testThreadId, request);
            }
            finally
            {
                await CleanupFileAsync(file);
            }

            Assert.IsNotNull(message);
            Assert.AreEqual(testThreadId, message.ThreadId);
            testMessageId = message.Id;
        }

        [Test]
        public async Task Test_04_02_ListMessages()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var message1 = await OpenAIClient.ThreadsEndpoint.CreateMessageAsync(testThreadId, new CreateMessageRequest("Test content"));
            Assert.IsNotNull(message1);
            var message2 = await OpenAIClient.ThreadsEndpoint.CreateMessageAsync(testThreadId, new CreateMessageRequest("Test content 2"));
            Assert.IsNotNull(message2);
            var list = await OpenAIClient.ThreadsEndpoint.ListMessagesAsync(testThreadId);
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var message in list.Items)
            {
                var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveMessageAsync(testThreadId, message);
                Assert.NotNull(retrieved);
            }
        }

        [Test]
        public async Task Test_04_03_ModifyMessage()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var modified = await OpenAIClient.ThreadsEndpoint.ModifyMessageAsync(
                testThreadId,
                testMessageId,
                new Dictionary<string, string>
                {
                    ["test"] = "04_03"
                });
            Assert.IsNotNull(modified);
            Assert.IsNotNull(modified.Metadata);
        }

        [Test]
        public async Task Test_04_04_ListMessageFiles()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            var file1 = await CreateTestFileAsync("test_1.txt");
            var file2 = await CreateTestFileAsync("test_2.txt");
            try
            {
                var createRequest = new CreateMessageRequest("Test content with files", new[] { file1.Id, file2.Id });
                var message = await OpenAIClient.ThreadsEndpoint.CreateMessageAsync(testThreadId, createRequest);
                var list = await OpenAIClient.ThreadsEndpoint.ListFilesAsync(message.ThreadId, message.Id);

                Assert.IsNotNull(list);
                Assert.AreEqual(2, list.Items.Count);

                foreach (var file in list.Items)
                {
                    var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveFileAsync(message, file);
                    Assert.IsNotNull(retrieved);
                }
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
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var isDeleted = await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(testThreadId);
            Assert.IsTrue(isDeleted);
            Console.WriteLine($"Deleted thread {testThreadId}");
        }

        private async Task<FileData> CreateTestFileAsync(string filePath)
        {
            await File.WriteAllTextAsync(filePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(filePath));
            var file = await OpenAIClient.FilesEndpoint.UploadFileAsync(filePath, "assistants");
            File.Delete(filePath);
            Assert.IsFalse(File.Exists(filePath));
            return file;
        }

        private async Task CleanupFileAsync(FileData file)
        {
            var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
            Assert.IsTrue(isDeleted);
        }
    }
}