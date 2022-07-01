using OpenAI.Endpoints;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI
{
    /// <summary>
    /// Entry point to the Azure OpenAI API, handling auth and allowing access to the various API endpoints
    /// See documentation here: https://docs.microsoft.com/en-us/azure/cognitive-services/openai/reference
    /// </summary>
    public class AzureOpenAIClient : BaseOpenAIClient, ICompletionEndpoint
    {

        /// <summary>
        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="endpointName"> The endpiont of the api, usually takes the form of https://xxxxx.openai.azure.com</param>
        /// <param name="authentication">The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="OpenAIAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.</param>
        /// <param name="defaultEngine">The <see cref="Engine"/>/model to use for API calls,        
        /// defaulting to <see cref="Engine.Davinci"/> if not specified.</param>
        /// <param name="version"/> Version of the api</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        public AzureOpenAIClient(string endpointName, OpenAIAuthentication authentication = null, Engine defaultEngine = null, string version = "2022-06-01-preview") : base(version, authentication, defaultEngine)
        {
            EndpointName = endpointName;            
            CompletionEndpoint = new CompletionEndpoint(this, engine => $"{BaseUrl}deployments/{engine.EngineName}/completions?api-version={Version}");
        }

        /// <summary>
        /// The BaseUrl of the Azure Open AI Service
        /// </summary>
        public string EndpointName { get; private set; }

        /// <summary>
        /// The base url for the Azure OpenAI service
        /// This url takes the form of https://xxx.openai.azure.com/
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return $"{EndpointName}openai/";
            }
        }    

        /// <summary>
        /// Internal add authorization method, adds the api-key to the header for the Azure OpenAI endpoint
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="openAIAuthentication"></param>
        internal override void AddAuthorization(HttpRequestHeaders headers, OpenAIAuthentication openAIAuthentication)
        {
            headers.Add("api-key", Auth.ApiKey);
        }               

        public CompletionEndpoint CompletionEndpoint { get; }
    }
}
