using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Authentication;


//[assembly: InternalsVisibleTo("OpenAI-DotNet-Tests")]

namespace OpenAI_DotNet
{
    /// <summary>
    /// Entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
    /// </summary>
    public class OpenAI
    {
        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="engine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        public OpenAI(Engine engine) : this(null, engine) { }

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="auth">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="Authentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="engine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        public OpenAI(Authentication auth = null, Engine engine = null)
        {
            Auth = auth ?? Authentication.Default;

            if (Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/OkGoDoIt/OpenAI-API-dotnet#authentication for details.");
            }

            DefaultEngine = engine ?? Engine.Default;
            CompletionEndpoint = new CompletionEndpoint(this);
            EnginesEndpoint = new EnginesEndpoint(this);
            SearchEndpoint = new SearchEndpoint(this);
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Auth.ApiKey);
            Client.DefaultRequestHeaders.Add("User-Agent", "dotnet_openai_api");
            Version = 1;
        }

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public Authentication Auth { get; }

        /// <summary>
        /// Specifies which <see cref="Engine"/>/model to use for API calls
        /// </summary>
        public Engine DefaultEngine { get; set; }

        private int version;

        /// <summary>
        /// Specifies which version of the API to use.
        /// </summary>
        public int Version
        {
            get => version;
            set
            {
                version = value;
                BaseUrl = $"https://api.openai.com/v{version}/";
            }
        }

        /// <summary>
        /// Text generation is the core function of the API.
        /// You give the API a prompt, and it generates a completion.
        /// The way you “program” the API to do a task is by simply describing the task in plain english
        /// or providing a few written examples. This simple approach works for a wide range of use cases,
        /// including summarization, translation, grammar correction, question answering, chatbots, composing emails,
        /// and much more (see the prompt library for inspiration).
        /// </summary>
        public CompletionEndpoint CompletionEndpoint { get; }

        /// <summary>
        /// The API endpoint for querying available Engines/models
        /// </summary>
        public EnginesEndpoint EnginesEndpoint { get; }

        /// <summary>
        /// The API lets you do semantic search over documents. This means that you can provide a query,
        /// such as a natural language question or a statement, and find documents that answer the question
        /// or are semantically related to the statement. The “documents” can be words, sentences, paragraphs
        /// or even longer documents. For example, if you provide documents "White House", "hospital", "school"
        /// and query "the president", you’ll get a different similarity score for each document.
        /// The higher the similarity score, the more semantically similar the document is to the query
        /// (in this example, “White House” will be most similar to “the president”).
        /// </summary>
        public SearchEndpoint SearchEndpoint { get; }

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        internal string BaseUrl { get; set; }
    }
}
