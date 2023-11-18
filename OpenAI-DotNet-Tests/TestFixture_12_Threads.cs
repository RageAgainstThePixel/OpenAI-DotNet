using System;
using NUnit.Framework;
using OpenAI.Threads;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;

namespace OpenAI.Tests
{
    internal class TestFixture_12_Threads : AbstractTestFixture
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

        private static string testThreadId;

        [Test]
        public async Task Test_01_CreateThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
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
        public async Task Test_04_DeleteThread()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(testThreadId));
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var isDeleted = await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(testThreadId);
            Assert.IsTrue(isDeleted);
            Console.WriteLine($"Deleted thread {testThreadId}");
        }
    }
}