using OpenAI.Endpoints;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI
{
    /// <summary>
    /// Entry point to the OpenAI API, handling auth and allowing access to the various API endpoints
    /// </summary>
    public class OpenAIClient : BaseOpenAIClient, IEnginerEndpoint, ICompletionEndpoint, ISearchEndpoint, IClassificationEndpoint
    {

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="version">The version for the api, defaults to 1 </param>
        /// <param name="defaultEngine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public OpenAIClient(Engine defaultEngine, string version = "1") : this(null, defaultEngine, version) { }

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="authentication">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAIAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="engine">The <see cref="Engine"/>/model to use for API calls,
        /// <paramref name="version"/> Version of the api, defaults to 1 </param>
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public OpenAIClient(OpenAIAuthentication authentication = null, Engine engine = null, string version = "1") : base(version, authentication, engine)
        {

            DefaultEngine = engine ?? Engine.Default;
            EnginesEndpoint = new EnginesEndpoint(this, engine => $"{BaseUrl}engines");
            CompletionEndpoint = new CompletionEndpoint(this, engine => $"{BaseUrl}engines/{engine.EngineName}/completions");
            SearchEndpoint = new SearchEndpoint(this, engine => $"{BaseUrl}engines/{engine.EngineName}/search");
            ClassificationEndpoint = new ClassificationEndpoint(this, engine => $"{BaseUrl}classifications");
        }

        /// <summary>
        /// Specifies which version of the API to use.
        /// </summary>
        public string BaseUrl
        {
            get => $"https://api.openai.com/v{Version}/";
        }
        /// <summary>
        /// Internal function to generate proper authentication header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="auth"></param>
        internal override void AddAuthorization(HttpRequestHeaders headers, OpenAIAuthentication auth)
        {
            headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.ApiKey);
        }

        public EnginesEndpoint EnginesEndpoint { get; }

        public CompletionEndpoint CompletionEndpoint { get; }

        public SearchEndpoint SearchEndpoint { get; }

        public ClassificationEndpoint ClassificationEndpoint { get; }

    }
}
