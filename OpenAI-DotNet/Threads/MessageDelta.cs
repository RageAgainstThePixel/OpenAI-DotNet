// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class MessageDelta
    {
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        [JsonInclude]
        [JsonPropertyName("content")]
        public IReadOnlyList<Content> Content { get; private set; }
    }
}
