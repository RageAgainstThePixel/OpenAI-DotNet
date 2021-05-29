using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI_DotNet
{
    /// <summary>
    /// Entry point to the OpenAI API, handling auth and allowing access to the various API endpoints
    /// </summary>
    public class OpenAI
    {
        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="engine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public OpenAI(Engine engine) : this(null, engine) { }

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="auth">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="Authentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="engine">The <see cref="Engine"/>/model to use for API calls,
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public OpenAI(Authentication auth = null, Engine engine = null)
        {
            Auth = auth ?? Authentication.Default;

            if (Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/StephenHodgson/OpenAI-DotNet#authentication for details.");
            }

            Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Auth.ApiKey);
            Client.DefaultRequestHeaders.Add("User-Agent", "dotnet_openai_api");
            Version = 1;
            JsonSerializationOptions = new JsonSerializerOptions { IgnoreNullValues = true };

            DefaultEngine = engine ?? Engine.Default;
            EnginesEndpoint = new EnginesEndpoint(this);
            CompletionEndpoint = new CompletionEndpoint(this);
            SearchEndpoint = new SearchEndpoint(this);
            ClassificationEndpoint = new ClassificationEndpoint(this);
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
        /// The API endpoint for querying available Engines/models
        /// </summary>
        public EnginesEndpoint EnginesEndpoint { get; }

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
        /// This endpoint lets you do semantic search over documents. This means that you can provide a query,
        /// such as a natural language question or a statement, and find documents that answer the question
        /// or are semantically related to the statement. The “documents” can be words, sentences, paragraphs
        /// or even longer documents. For example, if you provide documents "White House", "hospital", "school"
        /// and query "the president", you’ll get a different similarity score for each document.
        /// The higher the similarity score, the more semantically similar the document is to the query
        /// (in this example, “White House” will be most similar to “the president”).
        /// </summary>
        public SearchEndpoint SearchEndpoint { get; }

        /// <summary>
        /// This endpoint provides the ability to leverage a labeled set of examples without fine-tuning and can be
        /// used for any text-to-label task. By avoiding fine-tuning, it eliminates the need for hyper-parameter tuning.
        /// The endpoint serves as an "autoML" solution that is easy to configure, and adapt to changing label schema.
        /// Up to 200 labeled examples can be provided at query time. Given a query and a set of labeled examples,
        /// the model will predict the most likely label for the query. Useful as a drop-in replacement for any ML
        /// classification or text-to-label task.
        /// </summary>
        public ClassificationEndpoint ClassificationEndpoint { get; }

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        /// <summary>
        /// The base url to use when making calls to the API.
        /// </summary>
        internal string BaseUrl { get; private set; }

        /// <summary>
        /// The <see cref="JsonSerializationOptions"/> to use when making calls to the API.
        /// </summary>
        internal JsonSerializerOptions JsonSerializationOptions { get; }
    }
}
