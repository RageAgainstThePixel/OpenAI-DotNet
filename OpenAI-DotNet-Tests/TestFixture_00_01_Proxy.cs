// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_00_01_Proxy : AbstractTestFixture
    {
        [Test]
        public void Test_00_Proxy_Host_And_Ports()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettings = Path.Combine(projectDir, "Properties", "launchSettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile(launchSettings, optional: false)
                .Build();

            var applicationUrl = config["profiles:OpenAI_DotNet_Tests_Proxy:applicationUrl"];
            Assert.IsNotNull(applicationUrl);
            var hosts = applicationUrl.Split(";");
            var https = hosts[0];
            Assert.AreEqual("https://localhost:7133", https);
            var http = hosts[1];
            Assert.AreEqual("http://localhost:5105", http);

            var httpsUri = new Uri(https);
            Assert.AreEqual("localhost", httpsUri.Host);
            Assert.AreEqual(7133, httpsUri.Port);

            var httpUri = new Uri(http);
            Assert.AreEqual("localhost", httpUri.Host);
            Assert.AreEqual(5105, httpUri.Port);

            Assert.AreEqual(httpsUri.Host, HttpClient.BaseAddress?.Host);
            Assert.AreEqual(httpsUri.Port, HttpClient.BaseAddress?.Port);
        }

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
            var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new("https://localhost:7777")
            });
            var settings = new OpenAIClientSettings(domain: httpClient.BaseAddress?.Authority);
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
