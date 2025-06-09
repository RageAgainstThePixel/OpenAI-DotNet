// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class WebSearchPreviewTool : ITool
    {
        public static implicit operator Tool(WebSearchPreviewTool webSearchPreviewTool) => new(webSearchPreviewTool as ITool);

        public WebSearchPreviewTool() { }

        public WebSearchPreviewTool(SearchContextSize searchContextSize = 0, UserLocation userLocation = null)
        {
            SearchContextSize = searchContextSize;
            UserLocation = userLocation;
        }

        [JsonPropertyName("type")]
        public string Type => "web_search_preview";

        /// <summary>
        /// High level guidance for the amount of context window space to use for the search. One of low, medium, or high. medium is the default.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("search_context_size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SearchContextSize SearchContextSize { get; private set; }

        /// <summary>
        /// The user's location.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_location")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserLocation UserLocation { get; private set; }
    }
}
