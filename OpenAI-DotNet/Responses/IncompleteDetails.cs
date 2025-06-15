// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class IncompleteDetails
    {
        [JsonInclude]
        [JsonPropertyName("reason")]
        public IncompleteReason Reason { get; private set; }
    }
}
