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

            var violationResponse = await OpenAIClient.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(violationResponse);

            var response = await OpenAIClient.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
            Assert.IsNotNull(response);
            Console.WriteLine(response.Results?[0]?.Scores?.ToString());
        }
    }
}
