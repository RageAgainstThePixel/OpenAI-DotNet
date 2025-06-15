// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class ComputerToolCall : BaseResponse, IResponseItem
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("status")]
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// An identifier used when responding to the tool call with output.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("call_id")]
        public string CallId { get; private set; }

        /// <summary>
        /// An action for the computer use tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public IComputerAction Action { get; private set; }

        /// <summary>
        /// The pending safety checks for the computer call.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("pending_safety_checks")]
        public IReadOnlyList<ComputerToolCallSafetyCheck> PendingSafetyChecks { get; private set; }

        /// <summary>
        /// A computer screenshot image used with the computer use tool.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output")]
        public ComputerScreenShot ComputerScreenShot { get; private set; }
    }
}
