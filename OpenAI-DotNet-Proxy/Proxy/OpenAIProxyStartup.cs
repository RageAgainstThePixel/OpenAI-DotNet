using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace OpenAI.Proxy
{
    /// <summary>
    /// Used in ASP.NET Core WebApps to start your own OpenAI web api proxy.
    /// </summary>
    public class OpenAIProxyStartup
    {
        private OpenAIClient openAIClient;
        private IAuthenticationFilter authenticationFilter;

        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            openAIClient = app.ApplicationServices.GetRequiredService<OpenAIClient>();
            authenticationFilter = app.ApplicationServices.GetRequiredService<IAuthenticationFilter>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/health", HealthEndpoint);
                endpoints.Map($"{openAIClient.OpenAIClientSettings.BaseRequest}{{**endpoint}}", HandleRequest);
            });
        }

        /// <summary>
        /// Creates a new <see cref="IHost"/> that acts as a proxy web api for OpenAI.
        /// </summary>
        /// <typeparam name="T"><see cref="IAuthenticationFilter"/> type to use to validate your custom issued tokens.</typeparam>
        /// <param name="args">Startup args.</param>
        /// <param name="openAIClient"><see cref="OpenAIClient"/> with configured <see cref="OpenAIAuthentication"/> and <see cref="OpenAIClientSettings"/>.</param>
        public static IHost CreateDefaultHost<T>(string[] args, OpenAIClient openAIClient) where T : class, IAuthenticationFilter
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<OpenAIProxyStartup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.AllowSynchronousIO = false;
                        options.Limits.MinRequestBodyDataRate = null;
                        options.Limits.MinResponseDataRate = null;
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                        options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(openAIClient);
                    services.AddSingleton<IAuthenticationFilter, T>();
                }).Build();
        }

        private static async Task HealthEndpoint(HttpContext context)
        {
            // Respond with a 200 OK status code and a plain text message
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("OK");
        }

        /// <summary>
        /// Handles incoming requests, validates authentication, and forwards the request to OpenAI API
        /// </summary>
        private async Task HandleRequest(HttpContext httpContext, string endpoint)
        {
            try
            {
                authenticationFilter.ValidateAuthentication(httpContext.Request.Headers);

                var method = new HttpMethod(httpContext.Request.Method);
                var uri = new Uri(string.Format(openAIClient.OpenAIClientSettings.BaseRequestUrlFormat, $"{endpoint}{httpContext.Request.QueryString}"));
                var openAIRequest = new HttpRequestMessage(method, uri);

                openAIRequest.Content = new StreamContent(httpContext.Request.Body);

                if (httpContext.Request.ContentType != null)
                {
                    openAIRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(httpContext.Request.ContentType);
                }

                var proxyResponse = await openAIClient.Client.SendAsync(openAIRequest, HttpCompletionOption.ResponseHeadersRead);
                httpContext.Response.StatusCode = (int)proxyResponse.StatusCode;

                foreach (var header in proxyResponse.Headers)
                {
                    httpContext.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in proxyResponse.Content.Headers)
                {
                    httpContext.Response.Headers[header.Key] = header.Value.ToArray();
                }

                httpContext.Response.ContentType = proxyResponse.Content.Headers.ContentType?.ToString() ?? string.Empty;

                if (httpContext.Response.ContentType.Equals("text/event-stream"))
                {
                    var stream = await proxyResponse.Content.ReadAsStreamAsync();
                    await WriteServerStreamEventsAsync(httpContext, stream);
                }
                else
                {
                    await proxyResponse.Content.CopyToAsync(httpContext.Response.Body);
                }
            }
            catch (AuthenticationException authenticationException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync(authenticationException.Message);
            }
            catch (Exception e)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsync(e.Message);
            }
        }

        private static async Task WriteServerStreamEventsAsync(HttpContext httpContext, Stream contentStream)
        {
            var responseStream = httpContext.Response.Body;
            await contentStream.CopyToAsync(responseStream, httpContext.RequestAborted);
            await responseStream.FlushAsync(httpContext.RequestAborted);
        }
    }
}
