// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace OpenAI.Proxy
{
    [Obsolete("Use OpenAIProxy")]
    public class OpenAIProxyStartup
    {
        private OpenAIProxy openAIProxy;

        private OpenAIProxy OpenAIProxy => openAIProxy ??= new OpenAIProxy();

        public void ConfigureServices(IServiceCollection services)
            => OpenAIProxy.ConfigureServices(services);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => OpenAIProxy.Configure(app, env);

        public static IHost CreateDefaultHost<T>(string[] args, OpenAIClient openAIClient)
            where T : class, IAuthenticationFilter => OpenAIProxy.CreateDefaultHost<T>(args, openAIClient);

        public static WebApplication CreateWebApplication<T>(string[] args, OpenAIClient openAIClient)
            where T : class, IAuthenticationFilter => OpenAIProxy.CreateWebApplication<T>(args, openAIClient);
    }
}
