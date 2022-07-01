using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI
{
    public abstract class BaseOpenAIClient
    {

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="version">The version of the underlying endpoint</param>
        /// <param name="authentication">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAIAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="defaultEngine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public BaseOpenAIClient(string version, OpenAIAuthentication authentication = null, Engine defaultEngine = null)
        {
            Version = version;
            Auth = authentication ?? OpenAIAuthentication.Default;

            if (Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/RageAgainstThePixel/OpenAI-DotNet#authentication for details.");
            }

            Client = new HttpClient();
            AddAuthorization(Client.DefaultRequestHeaders, Auth);
            Client.DefaultRequestHeaders.Add("User-Agent", "dotnet_openai_api");
            JsonSerializationOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            DefaultEngine = defaultEngine ?? Engine.Default;
        }

        /// <summary>
        /// Provides support to add auth key for each type
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="openAIAuthentication"></param>
        internal abstract void AddAuthorization(HttpRequestHeaders headers, OpenAIAuthentication openAIAuthentication);

        /// <summary>
        /// The version of the Api
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public OpenAIAuthentication Auth { get; private set; }

        /// <summary>
        /// Specifies which <see cref="Engine"/>/model to use for API calls
        /// </summary>
        public Engine DefaultEngine { get; set; }

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        /// <summary>
        /// The <see cref="JsonSerializationOptions"/> to use when making calls to the API.
        /// </summary>
        internal JsonSerializerOptions JsonSerializationOptions { get; }

    }
}
