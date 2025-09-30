// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class CreateConversationRequest
    {
        public CreateConversationRequest() { }

        public CreateConversationRequest(IResponseItem item, IReadOnlyDictionary<string, string> metadata = null)
            : this([item], metadata)
        {
        }

        public CreateConversationRequest(IEnumerable<IResponseItem> items, IReadOnlyDictionary<string, string> metadata = null)
        {
            Items = items;
            Metadata = metadata;
        }

        /// <summary>
        /// Initial items to include in the conversation context. You may add up to 20 items at a time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("items")]
        public IEnumerable<IResponseItem> Items { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format,
        /// and querying for objects via API or the dashboard. Keys are strings with a maximum length of 64 characters.
        /// Values are strings with a maximum length of 512 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }
    }
}
