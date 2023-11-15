using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Threads;

namespace OpenAI.Tests
{
    internal class TestFixture_13_Theads : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_CreateThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);

            var request = new CreateThreadRequest
            {
                Messages = new List<CreateThreadRequest.Message>()
                {
                    new CreateThreadRequest.Message("Test message")
                },
                Metadata = new Dictionary<string, object>
                {
                    ["text"] = "test"
                }
            };

            var result = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual("thread", result.Object);

            Assert.IsNotNull(result.Metadata);
            Assert.Contains("text", result.Metadata.Keys);
            Assert.AreEqual("test", result.Metadata["text"]);
        }

        [Test]
        public async Task Test_02_RetrieveThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);

            var createThreadRequest = new CreateThreadRequest
            {
                Metadata = new Dictionary<string, object>
                {
                    ["text"] = "test"
                }
            };

            var created = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(createThreadRequest);

            var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveThreadAsync(created.Id);

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(created.Id, retrieved.Id);
            Assert.IsNotNull(retrieved.Metadata);
            Assert.Contains("text", retrieved.Metadata.Keys);
            Assert.AreEqual("test", retrieved.Metadata["text"]);
        }

        [Test]
        public async Task Test_03_ModifyThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);

            var created = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var newMetadata = new Dictionary<string, object>
            {
                ["text"] = "test2"
            };

            var modified = await OpenAIClient.ThreadsEndpoint.ModifyThreadAsync(created.Id, newMetadata);

            Assert.IsNotNull(modified);
            Assert.AreEqual(created.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("text", modified.Metadata.Keys);
            Assert.AreEqual("test2", modified.Metadata["text"]);
        }

        [Test]
        public async Task Test_04_DeleteThread()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);

            var created = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new CreateThreadRequest());

            var isDeleted = await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(created.Id);

            Assert.IsTrue(isDeleted);
        }
    }
}