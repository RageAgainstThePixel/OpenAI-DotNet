using NUnit.Framework;
using System.Threading.Tasks;

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

            var response = await OpenAIClient.ModerationsEndpoint.GetModerationAsync("I love you");
            Assert.IsFalse(response);
        }
    }
}
