// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal sealed class ImagesResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("created")]
        public int Created { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<ImageResult> Results { get; private set; }
    }
}
