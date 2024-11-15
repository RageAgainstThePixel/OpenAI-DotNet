// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class StatusDetails
    {
        /// <summary>
        /// The type of error that caused the response to fail, corresponding with the status field (cancelled, incomplete, failed).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The reason the Response did not complete.
        /// For a cancelled Response, one of turn_detected (the server VAD detected a new start of speech) or
        /// client_cancelled (the client sent a cancel event).
        /// For an incomplete Response, one of max_output_tokens or content_filter
        /// (the server-side safety filter activated and cut off the response).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("reason")]
        public string Reason { get; private set; }

        /// <summary>
        /// A description of the error that caused the response to fail, populated when the status is failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Error Error { get; private set; }
    }
}
