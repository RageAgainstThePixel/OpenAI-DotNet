using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImagesResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("created")]
        public int Created { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<ImageResult> Data { get; private set; }
    }
}
