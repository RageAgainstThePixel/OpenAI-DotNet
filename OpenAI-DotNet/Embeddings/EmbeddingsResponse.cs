// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Embeddings
{
    public sealed class EmbeddingsResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<Datum> Data { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("usage")]
        public Usage Usage { get; private set; }
    }
}
