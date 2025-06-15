// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A pending safety check for the computer call.
    /// </summary>
    public sealed class ComputerToolCallSafetyCheck
    {
        /// <summary>
        /// The ID of the pending safety check.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The type of the pending safety check.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }

        /// <summary>
        /// Details about the pending safety check.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("message")]
        public string Message { get; private set; }
    }
}
