// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Moderations;
using System;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_10_Moderations : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Moderate()
        {
            Assert.IsNotNull(OpenAIClient.ModerationsEndpoint);
            var isViolation = await OpenAIClient.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(isViolation);
        }

        [Test]
        public async Task Test_02_Moderate_Scores()
        {
            Assert.IsNotNull(OpenAIClient.ModerationsEndpoint);
            var response = await OpenAIClient.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
            Assert.IsNotNull(response);
            Console.WriteLine(response.Results?[0]?.Scores?.ToString());
        }

        [Test]
        public async Task Test_03_Moderation_Chunked()
        {
            Assert.IsNotNull(OpenAIClient.ModerationsEndpoint);
            var isViolation = await OpenAIClient.ModerationsEndpoint.GetModerationChunkedAsync("I don't want to kill them. I want to kill them. I want to kill them.", chunkSize: "I don't want to kill them.".Length, chunkOverlap: 4);
            Assert.IsTrue(isViolation);
        }
    }
}
