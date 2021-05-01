using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAI_DotNet
{
    /// <summary>
    /// The API endpoint for querying available Engines/models
    /// </summary>
    public class EnginesEndpoint
    {
        private class EngineList
        {
            [JsonPropertyName("data")]
            public List<Engine> Data { get; set; }
        }

        private readonly OpenAI api;

        /// <summary>
        /// Constructor of the api endpoint.
        /// Rather than instantiating this yourself, access it through an instance of <see cref="OpenAI"/> as <see cref="OpenAI.EnginesEndpoint"/>.
        /// </summary>
        /// <param name="api"></param>
        internal EnginesEndpoint(OpenAI api) => this.api = api;

        /// <summary>
        /// List all engines via the API
        /// </summary>
        /// <returns>Asynchronously returns the list of all <see cref="Engine"/>s</returns>
        public async Task<List<Engine>> GetEnginesAsync()
        {
            if (api.Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/OkGoDoIt/OpenAI-API-dotnet#authentication for details.");
            }

            api.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api.Auth.ApiKey);
            api.Client.DefaultRequestHeaders.Add("User-Agent", "dotnet_openai_api");

            var response = await api.Client.GetAsync(@"https://api.openai.com/v1/engines");
            var resultAsString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<EngineList>(resultAsString)?.Data;
            }

            throw new HttpRequestException($"{nameof(GetEnginesAsync)} Failed! HTTP status code: {response.StatusCode}. Content: {resultAsString}");
        }

        /// <summary>
        /// Get details about a particular Engine from the API, specifically properties such as
        /// <see cref="Engine.Owner"/> and <see cref="Engine.Ready"/>
        /// </summary>
        /// <param name="id">The id/name of the engine to get more details about</param>
        /// <returns>Asynchronously returns the <see cref="Engine"/> with all available properties</returns>
        public async Task<Engine> RetrieveEngineDetailsAsync(string id)
        {
            var response = await api.Client.GetAsync($"{api.BaseUrl}engines/{id}");

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<Engine>(await response.Content.ReadAsStringAsync());
            }

            throw new HttpRequestException($"{nameof(RetrieveEngineDetailsAsync)} Failed! HTTP status code: {response.StatusCode}");
        }
    }
}
