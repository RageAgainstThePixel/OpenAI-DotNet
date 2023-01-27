using NUnit.Framework;
using OpenAI.Edits;
using System;

namespace OpenAI.Tests
{
    internal class TestFixture_03_Edits
    {
        [Test]
        public void Test_1_GetBasicEdit()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EditsEndpoint);
            var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
            var result = api.EditsEndpoint.CreateEditAsync(request).Result;
            Assert.IsNotNull(result);
            Assert.NotNull(result.Choices);
            Assert.NotZero(result.Choices.Count);
            Console.WriteLine(result);
            Assert.IsTrue(result.ToString().Contains("week"));
        }
    }
}
