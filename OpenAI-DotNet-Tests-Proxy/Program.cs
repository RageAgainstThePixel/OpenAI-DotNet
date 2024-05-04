// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using OpenAI.Proxy;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace OpenAI.Tests.Proxy
{
    /// <summary>
    /// Example Web App Proxy API.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class Program
    {
        private const string TestUserToken = "aAbBcCdDeE123456789";

        private class AuthenticationFilter : AbstractAuthenticationFilter
        {
            public override void ValidateAuthentication(IHeaderDictionary request)
            {
                // You will need to implement your own class to properly test
                // custom issued tokens you've setup for your end users.
                if (!request.Authorization.ToString().Contains(TestUserToken))
                {
                    throw new AuthenticationException("User is not authorized");
                }
            }

            public override async Task ValidateAuthenticationAsync(IHeaderDictionary request)
            {
                await Task.CompletedTask; // remote resource call

                // You will need to implement your own class to properly test
                // custom issued tokens you've setup for your end users.
                if (!request.Authorization.ToString().Contains(TestUserToken))
                {
                    throw new AuthenticationException("User is not authorized");
                }
            }
        }

        public static void Main(string[] args)
        {
            var auth = OpenAIAuthentication.LoadFromEnv();
            var settings = new OpenAIClientSettings(/* your custom settings if using Azure OpenAI */);
            using var openAIClient = new OpenAIClient(auth, settings);
            OpenAIProxyStartup.CreateWebApplication<AuthenticationFilter>(args, openAIClient).Run();
        }
    }
}