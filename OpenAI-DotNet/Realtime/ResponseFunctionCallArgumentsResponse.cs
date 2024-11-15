// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ResponseFunctionCallArgumentsResponse : BaseRealtimeEvent, IServerEvent, IRealtimeEventStream
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
        /// The ID of the function call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("call_id")]
        public string CallId { get; private set; }

        /// <summary>
        /// The arguments delta as a JSON string.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Delta { get; private set; }

        [JsonInclude]
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; private set; }

        /// <summary>
        /// The final arguments as a JSON string.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonNode Arguments { get; private set; }

        [JsonIgnore]
        public bool IsDelta => Type.EndsWith("delta");

        [JsonIgnore]
        public bool IsDone => Type.EndsWith("done");

        public static implicit operator ToolCall(ResponseFunctionCallArgumentsResponse response)
            => new(response.CallId, response.Name, response.Arguments);
    }
}
