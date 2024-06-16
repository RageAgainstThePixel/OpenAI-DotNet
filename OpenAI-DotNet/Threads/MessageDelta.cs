// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class MessageDelta
    {
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("content")]
        public IReadOnlyList<Content> Content { get; private set; }

        /// <summary>
        /// Formats all of the <see cref="Content"/> items into a single string,
        /// putting each item on a new line.
        /// </summary>
        /// <returns><see cref="string"/> of all <see cref="Content"/>.</returns>
        public string PrintContent()
            => Content == null
                ? string.Empty
                : string.Join("\n", Content.Select(c => c?.ToString()));
    }
}
