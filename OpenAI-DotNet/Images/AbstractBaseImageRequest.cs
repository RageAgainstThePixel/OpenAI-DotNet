using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    /// <summary>
    /// Abstract base image class for making requests.
    /// </summary>
    public abstract class AbstractBaseImageRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numberOfResults">
        /// The number of images to generate. Must be between 1 and 10.
        /// </param>
        /// <param name="size">
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected AbstractBaseImageRequest(int numberOfResults = 1, ImageSize size = ImageSize.Large, ResponseFormat responseFormat = ResponseFormat.Url, string user = null)
        {
            Number = numberOfResults;

            Size = size switch
            {
                ImageSize.Small => "256x256",
                ImageSize.Medium => "512x512",
                ImageSize.Large => "1024x1024",
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };

            User = user;
            ResponseFormat = responseFormat;
        }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        [JsonPropertyName("n")]
        public int Number { get; }

        /// <summary>
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </summary>
        [JsonPropertyName("response_format")]
        [JsonConverter(typeof(JsonStringEnumConverter<ResponseFormat>))]
        public ResponseFormat ResponseFormat { get; }

        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        [JsonPropertyName("size")]
        public string Size { get; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; }
    }
}