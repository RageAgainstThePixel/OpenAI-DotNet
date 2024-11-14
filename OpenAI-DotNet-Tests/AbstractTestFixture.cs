// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;

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
            HttpClient.Timeout = TimeSpan.FromMinutes(3);
            OpenAIClient = new OpenAIClient(auth, settings, HttpClient)
            {
                EnableDebug = true
            };
        }

        private Uri GetBaseAddressFromLaunchSettings()
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
