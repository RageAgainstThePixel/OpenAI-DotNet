using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Moderations
{
    /// <summary>
    /// The moderation endpoint is a tool you can use to check whether content complies with OpenAI's content policy.
    /// Developers can thus identify content that our content policy prohibits and take action, for instance by filtering it.
    /// <see href="https://beta.openai.com/docs/api-reference/moderations"/>
    /// </summary>
    public sealed class ModerationsEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public ModerationsEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint()
            => $"{Api.BaseUrl}moderations";

        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy
        /// </summary>
        /// <returns>
        /// True, if the text has been flagged by the model as violating OpenAI's content policy.
        /// </returns>
        public async Task<bool> GetModerationAsync(string text)
        {
            var result = await CreateModerationAsync(new ModerationsRequest(text));

            if (result?.Results == null ||
                result.Results.Count == 0)
            {
                return false;
            }

            return result.Results.Any(moderationResult => moderationResult.Flagged);
        }

        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy
        /// </summary>
        /// <param name="request"><see cref="ModerationsRequest"/></param>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<ModerationsResponse> CreateModerationAsync(ModerationsRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request, Api.JsonSerializationOptions);
            var response = await Api.Client.PostAsync(GetEndpoint(), jsonContent.ToJsonStringContent());

            if (response.IsSuccessStatusCode)
            {
                var resultAsString = await response.Content.ReadAsStringAsync();
                var moderationResponse = JsonSerializer.Deserialize<ModerationsResponse>(resultAsString, Api.JsonSerializationOptions);

                if (moderationResponse == null)
                {
                    throw new HttpRequestException($"{nameof(CreateModerationAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {resultAsString}");
                }

                moderationResponse.SetResponseData(response.Headers);

                return moderationResponse;
            }

            throw new HttpRequestException($"{nameof(CreateModerationAsync)} Failed!  HTTP status code: {response.StatusCode}. Request body: {jsonContent}");
        }
    }
}
