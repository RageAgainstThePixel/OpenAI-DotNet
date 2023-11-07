using OpenAI.Models;
using System;
using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Images
{
    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    public sealed class ImageGenerationRequest
    {
        [Obsolete("Use new constructor")]
        public ImageGenerationRequest(string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null, ResponseFormat responseFormat = ResponseFormat.Url)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prompt">
        /// A text description of the desired image(s).
        /// The maximum length is 1000 characters for dall-e-2 and 4000 characters for dall-e-3.
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        /// <param name="numberOfResults">
        /// The number of images to generate.
        /// Must be between 1 and 10. For dall-e-3, only n=1 is supported.
        /// </param>
        /// <param name="quality">
        /// The quality of the image that will be generated.
        /// hd creates images with finer details and greater consistency across the image.
        /// This param is only supported for dall-e-3.
        /// </param>
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="size">
        /// The size of the generated images.
        /// Must be one of 256x256, 512x512, or 1024x1024 for dall-e-2.
        /// Must be one of 1024x1024, 1792x1024, or 1024x1792 for dall-e-3 models.
        /// </param>
        /// <param name="style">
        /// The style of the generated images.
        /// Must be one of vivid or natural.
        /// Vivid causes the model to lean towards generating hyper-real and dramatic images.
        /// Natural causes the model to produce more natural, less hyper-real looking images.
        /// This param is only supported for dall-e-3.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        public ImageGenerationRequest(
            string prompt,
            Model model = null,
            int numberOfResults = 1,
            string quality = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            string size = null,
            string style = null,
            string user = null)
        {
            Prompt = prompt;
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.DallE_2 : model;
            Number = numberOfResults;
            Quality = quality;
            ResponseFormat = responseFormat;
            Size = size;
            Style = style;
            User = user;
        }

        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// A text description of the desired image(s).
        /// The maximum length is 1000 characters for dall-e-2 and 4000 characters for dall-e-3.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; }

        /// <summary>
        /// The number of images to generate.
        /// Must be between 1 and 10. For dall-e-3, only n=1 is supported.
        /// </summary>
        [JsonPropertyName("n")]
        public int Number { get; }

        /// <summary>
        /// The quality of the image that will be generated.
        /// hd creates images with finer details and greater consistency across the image.
        /// This param is only supported for dall-e-3.
        /// </summary>
        [JsonPropertyName("quality")]
        public string Quality { get; }

        /// <summary>
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </summary>
        [JsonPropertyName("response_format")]
        [JsonConverter(typeof(JsonStringEnumConverter<ResponseFormat>))]
        public ResponseFormat ResponseFormat { get; }

        /// <summary>
        /// The size of the generated images.
        /// Must be one of 256x256, 512x512, or 1024x1024 for dall-e-2.
        /// Must be one of 1024x1024, 1792x1024, or 1024x1792 for dall-e-3 models.
        /// </summary>
        [JsonPropertyName("size")]
        public string Size { get; }

        /// <summary>
        /// The style of the generated images.
        /// Must be one of vivid or natural.
        /// Vivid causes the model to lean towards generating hyper-real and dramatic images.
        /// Natural causes the model to produce more natural, less hyper-real looking images.
        /// This param is only supported for dall-e-3.
        /// </summary>
        [JsonPropertyName("style")]
        public string Style { get; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; }
    }
}
