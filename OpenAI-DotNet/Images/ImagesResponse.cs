using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImagesResponse : BaseResponse
    {
        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("data")]
        public List<ImageResult> Data { get; set; }
    }
}
