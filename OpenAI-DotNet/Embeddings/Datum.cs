using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Embeddings
{
    public sealed class Datum
    {
        [JsonConstructor]
        public Datum(string @object, IReadOnlyList<double> embedding, int index)
        {
            Object = @object;
            Embedding = embedding;
            Index = index;
        }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("embedding")]
        public IReadOnlyList<double> Embedding { get; }

        [JsonPropertyName("index")]
        public int Index { get; }
    }
}
