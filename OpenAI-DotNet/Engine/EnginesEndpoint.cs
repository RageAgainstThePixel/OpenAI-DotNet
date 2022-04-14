using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAI
{
    /// <summary>
    /// The API endpoint for querying available Engines/models.
    /// <see href="https://beta.openai.com/docs/api-reference/engines"/>
    /// </summary>
    public class EnginesEndpoint : BaseEndPoint
    {
        private class EngineList
        {
            [JsonPropertyName("data")]
            public List<Engine> Data { get; set; }
        }

        /// <inheritdoc />
        internal EnginesEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint(Engine engine = null)
        {
            return $"{Api.BaseUrl}engines";
        }

        /// <summary>
        /// List all engines via the API
        /// </summary>
        /// <returns>Asynchronously returns the list of all <see cref="Engine"/>s</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<List<Engine>> GetEnginesAsync()
        {
            var response = await Api.Client.GetAsync(GetEndpoint());
            var resultAsString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<EngineList>(resultAsString)?.Data;
            }

            throw new HttpRequestException($"{nameof(GetEnginesAsync)} Failed! HTTP status code: {response.StatusCode}.");
        }

        [Obsolete("Use GetEngineDetailsAsync instead")]
        public async Task<Engine> RetrieveEngineDetailsAsync(string id)
        {
            return await GetEngineDetailsAsync(id);
        }

        /// <summary>
        /// Get the details about a particular Engine from the API, specifically properties such as
        /// <see cref="Engine.Owner"/> and <see cref="Engine.Ready"/>
        /// </summary>
        /// <param name="id">The id/name of the engine to get more details about</param>
        /// <returns>Asynchronously returns the <see cref="Engine"/> with all available properties</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<Engine> GetEngineDetailsAsync(string id)
        {
            var response = await Api.Client.GetAsync($"{GetEndpoint()}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<Engine>(await response.Content.ReadAsStringAsync());
            }

            throw new HttpRequestException($"{nameof(GetEngineDetailsAsync)} for {id} Failed! HTTP status code: {response.StatusCode}");
        }
    }
}
