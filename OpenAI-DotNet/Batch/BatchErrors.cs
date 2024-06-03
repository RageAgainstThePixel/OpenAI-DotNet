// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public sealed class BatchErrors
    {
        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<Error> Errors { get; private set; }
    }
}
