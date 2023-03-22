using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using OpenAI.Proxy;
using System.Security.Authentication;

namespace OpenAI.Tests.Proxy
{
    /// <summary>
    /// Example Web App Proxy API.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class Program
    {
        private const string TestUserToken = "aAbBcCdDeE123456789";

        // ReSharper disable once ClassNeverInstantiated.Local
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
        }

        public static void Main(string[] args)
        {
            var auth = OpenAIAuthentication.LoadFromEnv();
            var settings = new OpenAIClientSettings(/* your custom settings if using Azure OpenAI */);
            var openAIClient = new OpenAIClient(auth, settings);
            var proxy = OpenAIProxyStartup.CreateDefaultHost<AuthenticationFilter>(args, openAIClient);
            proxy.Run();
        }
    }
}