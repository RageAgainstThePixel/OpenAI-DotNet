// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// References an image URL in the content of a message.
    /// </summary>
    public sealed class ImageUrl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">
        /// The external URL of the image, must be a supported image types: jpeg, jpg, png, gif, webp.
        /// </param>
        /// <param name="detail">
        /// Specifies the detail level of the image if specified by the user.
        /// 'low' uses fewer tokens, you can opt in to high resolution using 'high'.
        /// </param>
        [JsonConstructor]
        public ImageUrl(string url, ImageDetail detail = ImageDetail.Auto)
        {
            Url = url;
            Detail = detail;
        }

        /// <summary>
        /// The external URL of the image, must be a supported image types: jpeg, jpg, png, gif, webp.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; private set; }

        /// <summary>
        /// Specifies the detail level of the image if specified by the user.
        /// 'low' uses fewer tokens, you can opt in to high resolution using 'high'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("detail")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageDetail Detail { get; private set; }
    }
}
