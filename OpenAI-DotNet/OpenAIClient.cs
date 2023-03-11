using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Completions;
using OpenAI.Edits;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// Entry point to the OpenAI API, handling auth and allowing access to the various API endpoints
    /// </summary>
    public sealed class OpenAIClient
    {
        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="openAIAuthentication">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAI.OpenAIAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="clientSettings">Optional, <see cref="OpenAIClientSettings"/> for specifying OpenAI deployments to Azure.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public OpenAIClient(OpenAIAuthentication openAIAuthentication = null, OpenAIClientSettings clientSettings = null)
        {
            OpenAIAuthentication = openAIAuthentication ?? OpenAIAuthentication.Default;
            OpenAIClientSettings = clientSettings ?? OpenAIClientSettings.Default;

            if (OpenAIAuthentication?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/RageAgainstThePixel/OpenAI-DotNet#authentication for details.");
            }

            Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("User-Agent", "OpenAI-DotNet");

            if (OpenAIClientSettings.ResourceName == OpenAIClientSettings.OpenAIDomain)
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAIAuthentication.ApiKey);
            }
            else
            {
                Client.DefaultRequestHeaders.Add("api-key", OpenAIAuthentication.ApiKey);
            }

            if (!string.IsNullOrWhiteSpace(OpenAIAuthentication.OrganizationId))
            {
                Client.DefaultRequestHeaders.Add("OpenAI-Organization", OpenAIAuthentication.OrganizationId);
            }

            JsonSerializationOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ModelsEndpoint = new ModelsEndpoint(this);
            CompletionsEndpoint = new CompletionsEndpoint(this);
            ChatEndpoint = new ChatEndpoint(this);
            EditsEndpoint = new EditsEndpoint(this);
            ImagesEndPoint = new ImagesEndpoint(this);
            EmbeddingsEndpoint = new EmbeddingsEndpoint(this);
            AudioEndpoint = new AudioEndpoint(this);
            FilesEndpoint = new FilesEndpoint(this);
            FineTuningEndpoint = new FineTuningEndpoint(this);
            ModerationsEndpoint = new ModerationsEndpoint(this);
        }

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        /// <summary>
        /// The <see cref="JsonSerializationOptions"/> to use when making calls to the API.
        /// </summary>
        internal JsonSerializerOptions JsonSerializationOptions { get; }

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public OpenAIAuthentication OpenAIAuthentication { get; }

        private OpenAIClientSettings OpenAIClientSettings { get; }

        /// <summary>
        /// The base url to use when making calls to the API.
        /// </summary>
        internal string BaseRequestUrl => OpenAIClientSettings.BaseRequestUrl;

        /// <summary>
        /// List and describe the various models available in the API.
        /// </summary>
        public ModelsEndpoint ModelsEndpoint { get; }

        /// <summary>
        /// Text generation is the core function of the API.
        /// You give the API a prompt, and it generates a completion.
        /// The way you “program” the API to do a task is by simply describing the task in plain english
        /// or providing a few written examples. This simple approach works for a wide range of use cases,
        /// including summarization, translation, grammar correction, question answering, chatbots, composing emails,
        /// and much more (see the prompt library for inspiration).
        /// </summary>
        public CompletionsEndpoint CompletionsEndpoint { get; }

        /// <summary>
        /// Given a chat conversation, the model will return a chat completion response.
        /// </summary>
        public ChatEndpoint ChatEndpoint { get; }

        /// <summary>
        /// Given a prompt and an instruction, the model will return an edited version of the prompt.
        /// </summary>
        public EditsEndpoint EditsEndpoint { get; }

        /// <summary>
        /// Given a prompt and/or an input image, the model will generate a new image.
        /// </summary>
        public ImagesEndpoint ImagesEndPoint { get; }

        /// <summary>
        /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.
        /// </summary>
        public EmbeddingsEndpoint EmbeddingsEndpoint { get; }

        /// <summary>
        /// Converts audio into text.
        /// </summary>
        public AudioEndpoint AudioEndpoint { get; }

        /// <summary>
        /// Files are used to upload documents that can be used with features like Fine-tuning.
        /// </summary>
        public FilesEndpoint FilesEndpoint { get; }

        /// <summary>
        /// Manage fine-tuning jobs to tailor a model to your specific training data.
        /// </summary>
        public FineTuningEndpoint FineTuningEndpoint { get; }

        /// <summary>
        /// The moderation endpoint is a tool you can use to check whether content complies with OpenAI's content policy. Developers can thus identify content that our content policy prohibits and take action, for instance by filtering it.
        /// </summary>
        public ModerationsEndpoint ModerationsEndpoint { get; }
    }
}
