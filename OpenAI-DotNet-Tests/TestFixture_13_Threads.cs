using NUnit.Framework;
using OpenAI.Threads;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_13_Threads : AbstractTestFixture
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
        public async Task Test_01_CreateThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            Assert.IsNotNull(thread);
            Assert.IsNotNull(thread.Metadata);
            Assert.IsNotEmpty(thread.Metadata);
            Assert.Contains("text", thread.Metadata.Keys.ToList());
            Assert.AreEqual("test", thread.Metadata["text"]);
        }

        [Test]
        public async Task Test_02_RetrieveThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            Assert.IsNotNull(thread);
            var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveThreadAsync(thread);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(thread.Id, retrieved.Id);
            Assert.IsNotNull(retrieved.Metadata);
            Assert.Contains("text", retrieved.Metadata.Keys.ToList());
            Assert.AreEqual("test", retrieved.Metadata["text"]);
        }

        [Test]
        public async Task Test_03_ModifyThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            Assert.IsNotNull(thread);
            var newMetadata = new Dictionary<string, string>
            {
                ["text"] = "test2"
            };
            var modified = await OpenAIClient.ThreadsEndpoint.ModifyThreadAsync(thread, newMetadata);
            Assert.IsNotNull(modified);
            Assert.AreEqual(thread.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("text", modified.Metadata.Keys.ToList());
            Assert.AreEqual("test2", modified.Metadata["text"]);
        }

        [Test]
        public async Task Test_04_DeleteThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThread);
            Assert.IsNotNull(thread);
            var isDeleted = await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(thread);
            Assert.IsTrue(isDeleted);
        }
    }
}