using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Edits;

namespace OpenAI.Embeddings;

public sealed class EmbeddingsResponse : BaseResponse
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("data")]
    public List<Datum> Data { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }
}