using NUnit.Framework;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal sealed class TestFixture_10_Moderations : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_ModerateAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.ModerationsEndpoint);

            var violationResponse = await this.OpenAIClient.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(violationResponse);

            var response = await this.OpenAIClient.ModerationsEndpoint.GetModerationAsync("I love you");
            Assert.IsFalse(response);
        }
    }
}
