using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TextFixture_11_Proxy : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Health()
        {
            var response = await HttpClient.GetAsync("/health");
            var responseAsString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[{response.StatusCode}] {responseAsString}");
            Assert.IsTrue(HttpStatusCode.OK == response.StatusCode);
        }

        [Test]
        public async Task Test_02_Client_Authenticated()
        {
            var models = await OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(models);
            Assert.IsNotEmpty(models);

            foreach (var model in models)
            {
                Console.WriteLine(model);
            }
        }

        [Test]
        public async Task Test_03_Client_Unauthenticated()
        {
            var webApplicationFactory = new TestProxyFactory();
            var httpClient = webApplicationFactory.CreateClient();
            var settings = new OpenAIClientSettings(domain: "localhost:7133");
            var auth = new OpenAIAuthentication("sess-invalid-token");
            var openAIClient = new OpenAIClient(auth, settings, httpClient);

            try
            {
                await openAIClient.ModelsEndpoint.GetModelsAsync();
            }
            catch (HttpRequestException httpRequestException)
            {
                // System.Net.Http.HttpRequestException : GetModelsAsync Failed! HTTP status code: Unauthorized | Response body: User is not authorized
                Assert.IsTrue(httpRequestException.StatusCode == HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
