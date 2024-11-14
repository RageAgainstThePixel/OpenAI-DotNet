// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
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
            var settings = new OpenAIClientSettings(domain: HttpClient.BaseAddress?.Authority);
            var auth = new OpenAIAuthentication("sess-invalid-token");
            var openAIClient = new OpenAIClient(auth, settings, HttpClient);

            try
            {
                await openAIClient.ModelsEndpoint.GetModelsAsync();
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine(httpRequestException);
                // System.Net.Http.HttpRequestException : GetModelsAsync Failed! HTTP status code: Unauthorized | Response body: User is not authorized
                Assert.AreEqual(HttpStatusCode.Unauthorized, httpRequestException.StatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        public async Task Test_04_Client_Websocket_Authentication()
        {
            using var websocket = new ClientWebSocket();

            foreach (var (key, value) in OpenAIClient.WebsocketHeaders)
            {
                websocket.Options.SetRequestHeader(key, value);
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var realtimeUri = new Uri(string.Format(OpenAIClient.OpenAIClientSettings.BaseWebSocketUrlFormat, "/realtime"));
            _ = websocket.ConnectAsync(realtimeUri, cts.Token);

            while (websocket.State != WebSocketState.Open)
            {
                cts.Token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            await Task.Delay(1000, cts.Token);
            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cts.Token);
        }
    }
}
