using NUnit.Framework;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_10_Moderations
    {
        [Test]
        public async Task Test_1_Moderate()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ModerationsEndpoint);

            var violationResponse = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(violationResponse);

            var response = await api.ModerationsEndpoint.GetModerationAsync("I love you");
            Assert.IsFalse(response);
        }
    }
}
