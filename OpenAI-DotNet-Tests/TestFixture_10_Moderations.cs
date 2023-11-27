using System;
using NUnit.Framework;
using System.Threading.Tasks;
using OpenAI.Moderations;

namespace OpenAI.Tests
{
    internal class TestFixture_10_Moderations : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_Moderate()
        {
            Assert.IsNotNull(OpenAIClient.ModerationsEndpoint);

            var isViolation = await OpenAIClient.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(isViolation);

            var isChunkedViolation = await OpenAIClient.ModerationsEndpoint.GetModerationChunkedAsync("I want to kill them.");
            Assert.IsTrue(isChunkedViolation);
            
            var response = await OpenAIClient.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
            Assert.IsNotNull(response);
            Console.WriteLine(response.Results?[0]?.Scores?.ToString());
        }

        [Test]
        public async Task Test_2_ModerationChunked()
        {
            Assert.IsNotNull(OpenAIClient.ModerationsEndpoint);

            var isChunkedViolation = await OpenAIClient.ModerationsEndpoint.GetModerationChunkedAsync(
                "I don't want to kill them. I want to kill them. I want to kill them.", chunkSize: "I don't want to kill them.".Length,
                chunkOverlap: 4);

            Assert.IsTrue(isChunkedViolation);
        }
    }
}
