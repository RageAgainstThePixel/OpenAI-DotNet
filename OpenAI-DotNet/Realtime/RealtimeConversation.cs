// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RealtimeConversation
    {
        /// <summary>
        /// The unique id of the conversation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, must be "realtime.conversation".
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        public static implicit operator string(RealtimeConversation conversation) => conversation?.Id;
    }
}
