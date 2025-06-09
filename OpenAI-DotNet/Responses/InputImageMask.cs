// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class InputImageMask
    {
        [JsonConstructor]
        public InputImageMask(string imageUrl = null, string fileId = null)
        {
            ImageUrl = imageUrl;
            FileId = fileId;
        }

        /// <summary>
        /// Base64-encoded mask image.
        /// </summary>
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; }

        /// <summary>
        /// File ID for the mask image.
        /// </summary>
        [JsonPropertyName("file_id")]
        public string FileId { get; }
    }
}
