// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
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
            HttpClient = webApplicationFactory.CreateClient();
            var domain = $"{HttpClient.BaseAddress?.Authority}:{HttpClient.BaseAddress?.Port}";
            var settings = new OpenAIClientSettings(domain: domain);
            var auth = new OpenAIAuthentication(TestUserToken);
            HttpClient.Timeout = TimeSpan.FromMinutes(3);
            OpenAIClient = new OpenAIClient(auth, settings, HttpClient)
            {
                EnableDebug = true
            };
        }
    }
}