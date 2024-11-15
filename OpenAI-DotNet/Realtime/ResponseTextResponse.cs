// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ResponseTextResponse : BaseRealtimeEvent, IServerEvent, IRealtimeEventStream
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; }

        /// <summary>
        /// The ID of the response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_id")]
        public string ResponseId { get; private set; }

        /// <summary>
        /// The ID of the item.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item_id")]
        public string ItemId { get; private set; }

        /// <summary>
        /// The index of the output item in the response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_index")]
        public int OutputIndex { get; private set; }

        /// <summary>
        /// The index of the content part in the item's content array.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_index")]
        public int ContentIndex { get; private set; }

        /// <summary>
        /// The text delta.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Delta { get; private set; }

        /// <summary>
        /// The final text content.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; private set; }

        [JsonIgnore]
        public bool IsDelta => Type.EndsWith("delta");

        [JsonIgnore]
        public bool IsDone => Type.EndsWith("done");

        public override string ToString()
            => IsDelta ? Delta : Text;

        public static implicit operator string(ResponseTextResponse response)
            => response?.ToString();
    }
}
