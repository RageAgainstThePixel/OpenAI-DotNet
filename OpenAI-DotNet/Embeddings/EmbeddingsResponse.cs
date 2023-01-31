using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Embeddings
{
    public sealed class EmbeddingsResponse : BaseResponse
    {
        [JsonConstructor]
        public EmbeddingsResponse(string @object, IReadOnlyList<Datum> data, string model, Usage usage)
        {
            Object = @object;
            Data = data;
            Model = model;
            Usage = usage;
        }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("data")]
        public IReadOnlyList<Datum> Data { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; }
    }
}
