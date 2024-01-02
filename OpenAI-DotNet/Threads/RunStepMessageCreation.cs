// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class RunStepMessageCreation
    {
        /// <summary>
        /// The ID of the message that was created by this run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message_id")]
        public string MessageId { get; private set; }
    }
}