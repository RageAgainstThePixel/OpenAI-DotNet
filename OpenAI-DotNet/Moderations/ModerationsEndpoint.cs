using OpenAI.Models;
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
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <param name="input">
        /// The input text to classify.
        /// </param>
        /// <param name="model">The default is text-moderation-latest which will be automatically upgraded over time.
        /// This ensures you are always using our most accurate model.
        /// If you use text-moderation-stable, we will provide advanced notice before updating the model.
        /// Accuracy of text-moderation-stable may be slightly lower than for text-moderation-latest.
        /// </param>
        /// <returns>
        /// True, if the text has been flagged by the model as violating OpenAI's content policy.
        /// </returns>
        public async Task<bool> GetModerationAsync(string input, Model model = null)
        {
            var result = await CreateModerationAsync(new ModerationsRequest(input, model)).ConfigureAwait(false);

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
            var response = await Api.Client.PostAsync(GetEndpoint(), jsonContent.ToJsonStringContent()).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
