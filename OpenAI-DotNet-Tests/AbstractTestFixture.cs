// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal abstract class AbstractTestFixture
    {
        protected class TestProxyFactory : WebApplicationFactory<Proxy.Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Development");
                base.ConfigureWebHost(builder);
            }
        }

        internal const string TestUserToken = "sess-aAbBcCdDeE123456789";

        protected readonly HttpClient HttpClient;

        protected readonly OpenAIClient OpenAIClient;

        protected AbstractTestFixture()
        {
            var webApplicationFactory = new TestProxyFactory();
            HttpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = GetBaseAddressFromLaunchSettings()
            });
            var settings = new OpenAIClientSettings(domain: HttpClient.BaseAddress?.Authority);
            var auth = new OpenAIAuthentication(TestUserToken);

            OpenAIClient = new OpenAIClient(auth, settings, HttpClient)
            {
                EnableDebug = true,
                CreateWebsocketAsync = CreateWebsocketAsync
            };

            return;

            async Task<WebSocket> CreateWebsocketAsync(Uri uri, CancellationToken cancellationToken)
            {
                var websocketClient = webApplicationFactory.Server.CreateWebSocketClient();
                websocketClient.ConfigureRequest = request =>
                {
                    foreach (var (key, value) in OpenAIClient.WebsocketHeaders)
                    {
                        request.Headers[key] = value;
                    }
                };
                var websocket = await websocketClient.ConnectAsync(uri, cancellationToken);
                return websocket;
            }
        }

        private static Uri GetBaseAddressFromLaunchSettings()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettings = Path.Combine(projectDir, "Properties", "launchSettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonFile(launchSettings, optional: false)
                .Build();
            var applicationUrl = config["profiles:OpenAI_DotNet_Tests_Proxy:applicationUrl"];
            if (string.IsNullOrEmpty(applicationUrl))
            {
                throw new InvalidOperationException("Base address not found in launchSettings.json");
            }
            var hosts = applicationUrl.Split(";");
            return new Uri(hosts[0]);
        }
    }
}
