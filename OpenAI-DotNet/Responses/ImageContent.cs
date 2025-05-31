// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class ImageContent : IResponseContent
    {
        public ImageContent() { }

        public ImageContent(string imageUrl = null, string fileId = null, ImageDetail detail = ImageDetail.Auto)
        {
            Type = ResponseContentType.InputImage;
            ImageUrl = imageUrl;
            FileId = fileId;
            Detail = detail;
        }

        public ImageContent(byte[] imageData, ImageDetail detail = ImageDetail.Auto)
        {
            Type = ResponseContentType.InputImage;
            Detail = detail;
            ImageUrl = $"data:image/png;base64,{Convert.ToBase64String(imageData)}";
        }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("detail")]
        public ImageDetail Detail { get; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("file_id")]
        public string FileId { get; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; }
    }
}
