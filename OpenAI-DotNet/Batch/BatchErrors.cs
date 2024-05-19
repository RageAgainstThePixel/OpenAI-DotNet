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
