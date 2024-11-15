// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Extensions;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.Threads;
using OpenAI.VectorStores;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    /// <summary>
    /// Entry point to the OpenAI API, handling auth and allowing access to the various API endpoints
    /// </summary>
    public sealed class OpenAIClient : IDisposable
    {
        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="openAIAuthentication">
        /// The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAIAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.
        /// </param>
        /// <param name="clientSettings">
        /// The API client settings for specifying OpenAI deployments to Azure, a proxy domain,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAIClientSettings.Default"/>,
        /// potentially loading from environment vars or from a config file.
        /// </param>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        /// <remarks>
        /// <see cref="OpenAIClient"/> implements <see cref="IDisposable"/> to manage the lifecycle of the resources it uses, including <see cref="HttpClient"/>.
        /// When you initialize <see cref="OpenAIClient"/>, it will create an internal <see cref="HttpClient"/> instance if one is not provided.
        /// This internal HttpClient is disposed of when OpenAIClient is disposed of.
        /// If you provide an external HttpClient instance to OpenAIClient, you are responsible for managing its disposal.
        /// </remarks>
        public OpenAIClient(OpenAIAuthentication openAIAuthentication = null, OpenAIClientSettings clientSettings = null, HttpClient client = null)
        {
            OpenAIAuthentication = openAIAuthentication ?? OpenAIAuthentication.Default;
            OpenAIClientSettings = clientSettings ?? OpenAIClientSettings.Default;

            if (string.IsNullOrWhiteSpace(OpenAIAuthentication?.ApiKey))
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/RageAgainstThePixel/OpenAI-DotNet#authentication for details.");
            }

            Client = SetupHttpClient(client);
            ModelsEndpoint = new ModelsEndpoint(this);
            ChatEndpoint = new ChatEndpoint(this);
            ImagesEndPoint = new ImagesEndpoint(this);
            EmbeddingsEndpoint = new EmbeddingsEndpoint(this);
            AudioEndpoint = new AudioEndpoint(this);
            FilesEndpoint = new FilesEndpoint(this);
            FineTuningEndpoint = new FineTuningEndpoint(this);
            ModerationsEndpoint = new ModerationsEndpoint(this);
            ThreadsEndpoint = new ThreadsEndpoint(this);
            AssistantsEndpoint = new AssistantsEndpoint(this);
            BatchEndpoint = new BatchEndpoint(this);
            VectorStoresEndpoint = new VectorStoresEndpoint(this);
            RealtimeEndpoint = new RealtimeEndpoint(this);
        }

        ~OpenAIClient() => Dispose(false);

        #region IDisposable

        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                if (!isCustomClient)
                {
                    Client?.Dispose();
                }

                isDisposed = true;
            }
        }

        #endregion IDisposable

        private bool isCustomClient;

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        /// <summary>
        /// The <see cref="JsonSerializationOptions"/> to use when making calls to the API.
        /// </summary>
        internal static JsonSerializerOptions JsonSerializationOptions { get; } = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverterFactory(),
                new RealtimeClientEventConverter(),
                new RealtimeServerEventConverter()
            },
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public OpenAIAuthentication OpenAIAuthentication { get; }

        /// <summary>
        /// The client settings for configuring Azure OpenAI or custom domain.
        /// </summary>
        internal OpenAIClientSettings OpenAIClientSettings { get; }

        /// <summary>
        /// Enables or disables debugging for all endpoints.
        /// </summary>
        public bool EnableDebug { get; set; }

        #region Endpoints

        /// <summary>
        /// List and describe the various models available in the API.
        /// You can refer to the Models documentation to understand which models are available for certain endpoints: <see href="https://platform.openai.com/docs/models/model-endpoint-compatibility"/>.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/models"/>
        /// </summary>
        public ModelsEndpoint ModelsEndpoint { get; }

        /// <summary>
        /// Given a chat conversation, the model will return a chat completion response.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/chat"/>
        /// </summary>
        public ChatEndpoint ChatEndpoint { get; }

        /// <summary>
        /// Given a prompt and/or an input image, the model will generate a new image.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/images"/>
        /// </summary>
        public ImagesEndpoint ImagesEndPoint { get; }

        /// <summary>
        /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.<br/>
        /// <see href="https://platform.openai.com/docs/guides/embeddings"/>
        /// </summary>
        public EmbeddingsEndpoint EmbeddingsEndpoint { get; }

        /// <summary>
        /// Transforms audio into text.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/audio"/>
        /// </summary>
        public AudioEndpoint AudioEndpoint { get; }

        /// <summary>
        /// Files are used to upload documents that can be used with features like Assistants, Fine-tuning, and Batch API.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/files"/>
        /// </summary>
        public FilesEndpoint FilesEndpoint { get; }

        /// <summary>
        /// Manage fine-tuning jobs to tailor a model to your specific training data.<br/>
        /// <see href="https://platform.openai.com/docs/guides/fine-tuning"/>
        /// </summary>
        public FineTuningEndpoint FineTuningEndpoint { get; }

        /// <summary>
        /// The moderation endpoint is a tool you can use to check whether content complies with OpenAI's content policy.
        /// Developers can thus identify content that our content policy prohibits and take action, for instance by filtering it.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/moderations"/>
        /// </summary>
        public ModerationsEndpoint ModerationsEndpoint { get; }

        /// <summary>
        /// Create threads that assistants can interact with.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/threads"/>
        /// </summary>
        public ThreadsEndpoint ThreadsEndpoint { get; }

        /// <summary>
        /// Build assistants that can call models and use tools to perform tasks.<br/>
        /// <see href="https://platform.openai.com/docs/api-reference/assistants"/>
        /// </summary>
        public AssistantsEndpoint AssistantsEndpoint { get; }

        /// <summary>
        /// Create large batches of API requests for asynchronous processing.
        /// The Batch API returns completions within 24 hours for a 50% discount.
        /// <see href="https://platform.openai.com/docs/api-reference/batch"/>
        /// </summary>
        public BatchEndpoint BatchEndpoint { get; }

        /// <summary>
        /// Vector stores are used to store files for use by the file_search tool.
        /// <see href="https://platform.openai.com/docs/api-reference/vector-stores"/>
        /// </summary>
        public VectorStoresEndpoint VectorStoresEndpoint { get; }

        public RealtimeEndpoint RealtimeEndpoint { get; }

        #endregion Endpoints

        private HttpClient SetupHttpClient(HttpClient client = null)
        {
            if (client == null)
            {
                client = new HttpClient(new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                });
            }
            else
            {
                isCustomClient = true;
            }

            client.DefaultRequestHeaders.Add("User-Agent", "OpenAI-DotNet");
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            if (OpenAIClientSettings.BaseRequestUrlFormat.Contains(OpenAIClientSettings.OpenAIDomain) &&
                (string.IsNullOrWhiteSpace(OpenAIAuthentication.ApiKey) ||
                 (!OpenAIAuthentication.ApiKey.Contains(AuthInfo.SecretKeyPrefix) &&
                  !OpenAIAuthentication.ApiKey.Contains(AuthInfo.SessionKeyPrefix))))
            {
                throw new InvalidCredentialException($"{OpenAIAuthentication.ApiKey} must start with '{AuthInfo.SecretKeyPrefix}'");
            }

            if (OpenAIClientSettings.UseOAuthAuthentication)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAIAuthentication.ApiKey);
            }
            else
            {
                client.DefaultRequestHeaders.Add("api-key", OpenAIAuthentication.ApiKey);
            }

            if (!string.IsNullOrWhiteSpace(OpenAIAuthentication.OrganizationId))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Organization", OpenAIAuthentication.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(OpenAIAuthentication.ProjectId))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Project", OpenAIAuthentication.ProjectId);
            }

            return client;
        }

        internal WebSocket CreateWebSocket(string url)
        {
            var websocket = new WebSocket(url, WebsocketHeaders);

            if (CreateWebsocketAsync != null)
            {
                websocket.CreateWebsocketAsync = CreateWebsocketAsync;
            }

            return websocket;
        }

        // used to create unit test proxy server
        internal Func<Uri, CancellationToken, Task<System.Net.WebSockets.WebSocket>> CreateWebsocketAsync = null;

        internal IReadOnlyDictionary<string, string> WebsocketHeaders => new Dictionary<string, string>
        {
            { "User-Agent", "OpenAI-DotNet" },
            { "OpenAI-Beta", "realtime=v1" },
            { "Authorization", $"Bearer {OpenAIAuthentication.ApiKey}" }
        };
    }
}
