// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Threads;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A tool call to run code.
    /// </summary>
    public sealed class CodeInterpreterToolCall : BaseResponse, IResponseItem
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
        /// The code to run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }

        /// <summary>
        /// The results of the code interpreter tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("results")]
        public IReadOnlyList<CodeInterpreterOutputs> Results { get; private set; }

        /// <summary>
        /// The ID of the container used to run the code.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("container_id")]
        public string ContainerId { get; private set; }
    }
}
