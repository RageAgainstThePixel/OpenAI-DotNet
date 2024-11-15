// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
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
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var realtimeUri = new Uri(string.Format(OpenAIClient.OpenAIClientSettings.BaseWebSocketUrlFormat, "echo"));
                Console.WriteLine(realtimeUri);
                using var websocket = await OpenAIClient.CreateWebsocketAsync.Invoke(realtimeUri, cts.Token);

                if (websocket.State != WebSocketState.Open)
                {
                    throw new Exception($"Failed to open WebSocket connection. Current state: {websocket.State}");
                }

                var data = new byte[1024];
                var buffer = new byte[1024 * 4];
                var random = new Random();
                random.NextBytes(data);
                await websocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, cts.Token);
                var receiveResult = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                Assert.AreEqual(WebSocketMessageType.Binary, receiveResult.MessageType);
                Assert.AreEqual(data.Length, receiveResult.Count);
                var receivedData = buffer[..receiveResult.Count];
                Assert.AreEqual(data.Length, receivedData.Length);
                Assert.AreEqual(data, receivedData);
                var message = $"hello world! {DateTime.UtcNow}";
                var messageData = Encoding.UTF8.GetBytes(message);
                await websocket.SendAsync(new ArraySegment<byte>(messageData), WebSocketMessageType.Text, true, cts.Token);
                receiveResult = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                Assert.AreEqual(WebSocketMessageType.Text, receiveResult.MessageType);
                Assert.AreEqual(messageData.Length, receiveResult.Count);
                Assert.AreEqual(messageData, buffer[..receiveResult.Count]);
                var decodedMessage = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                Assert.AreEqual(message, decodedMessage);
                await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test completed", cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
