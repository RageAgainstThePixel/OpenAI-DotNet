using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImagesResponse : BaseResponse
    {
        [JsonConstructor]
        public ImagesResponse(int created, IReadOnlyList<ImageResult> data)
        {
            Created = created;
            Data = data;
        }

        [JsonPropertyName("created")]
        public int Created { get; }

        [JsonPropertyName("data")]
        public IReadOnlyList<ImageResult> Data { get; }
    }
}
