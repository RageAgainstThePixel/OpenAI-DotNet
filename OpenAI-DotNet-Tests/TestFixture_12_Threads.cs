using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_12_Threads : AbstractTestFixture
    {
        private static ThreadResponse testThread;
        private static MessageResponse testMessage;

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
                ["test"] = "03"
            };
            var thread = await testThread.ModifyAsync(newMetadata);
            Assert.IsNotNull(thread);
            Assert.AreEqual(testThread.Id, thread.Id);
            Assert.IsNotNull(thread.Metadata);
            Console.WriteLine($"Modify thread {thread.Id} -> {thread.CreatedAt}");
        }

        [Test]
        public async Task Test_04_01_CreateMessage()
        {
            Assert.IsNotNull(testThread);
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
                var retrieved = await testThread.RetrieveMessageAsync(message);
                Assert.NotNull(retrieved);
            }
        }

        [Test]
        public async Task Test_04_03_ModifyMessage()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var metadata = new Dictionary<string, string>
            {
                ["test"] = "04_03"
            };
            var modified = await testMessage.ModifyAsync(metadata);
            Assert.IsNotNull(modified);
            Assert.IsNotNull(modified.Metadata);
            Assert.IsTrue(modified.Metadata["test"].Equals("04_03"));
        }

        [Test]
        public async Task Test_04_04_ListMessageFiles()
        {
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var file1 = await CreateTestFileAsync("test_1.txt");
            var file2 = await CreateTestFileAsync("test_2.txt");
            try
            {
                var createRequest = new CreateMessageRequest("Test content with files", new[] { file1.Id, file2.Id });
                var message = await testThread.CreateMessageAsync(createRequest);
                var list = await message.ListFilesAsync();

                Assert.IsNotNull(list);
                Assert.AreEqual(2, list.Items.Count);

                foreach (var file in list.Items)
                {
                    var retrieved = await message.RetrieveFileAsync(file);
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
            Assert.IsNotNull(testThread);
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var isDeleted = await testThread.DeleteAsync();
            Assert.IsTrue(isDeleted);
            Console.WriteLine($"Deleted thread {testThread.Id}");
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