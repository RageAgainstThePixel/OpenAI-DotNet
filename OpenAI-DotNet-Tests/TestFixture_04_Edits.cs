using NUnit.Framework;
using OpenAI.Edits;
using System;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_04_Edits
    {
        [Test]
        public async Task Test_1_GetBasicEdit()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EditsEndpoint);
            var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
            var result = await api.EditsEndpoint.CreateEditAsync(request);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Choices);
            Assert.NotZero(result.Choices.Count);
            Console.WriteLine(result);
            Assert.IsTrue(result.ToString().Contains("week"));
        }
    }
}
