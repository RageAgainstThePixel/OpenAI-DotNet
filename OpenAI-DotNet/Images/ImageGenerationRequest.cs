using System;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    public sealed class ImageGenerationRequest : AbstractBaseImageRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prompt">
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </param>
        /// <param name="numberOfResults">
        /// The number of images to generate. Must be between 1 and 10.
        /// </param>
        /// <param name="size">
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="responseFormat">
        /// The format in which the generated images are returned. Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        public ImageGenerationRequest(string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null, ResponseFormat responseFormat = Images.ResponseFormat.Url)
            : base(numberOfResults, size, responseFormat, user)
        {
            if (prompt.Length > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            }

            Prompt = prompt;

            if (numberOfResults is > 10 or < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            }
        }

        /// <summary>
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; }
    }
}
