using NUnit.Framework;
using OpenAI.Edits;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    [Obsolete]
    internal class TestFixture_04_Edits
    {
        [Test]
        public async Task Test_1_GetBasicEdit()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EditsEndpoint);
            EditResponse result = null;

            try
            {
                var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
                result = await api.EditsEndpoint.CreateEditAsync(request);
            }
            catch (HttpRequestException)
            {
                Assert.IsNull(result);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, $"Expected exception {nameof(HttpRequestException)} but got {e.GetType().Name}");
            }
        }
    }
}
