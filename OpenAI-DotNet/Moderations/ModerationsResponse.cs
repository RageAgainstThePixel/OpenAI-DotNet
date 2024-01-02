// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationsResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("results")]
        public IReadOnlyList<ModerationResult> Results { get; private set; }
    }
}
