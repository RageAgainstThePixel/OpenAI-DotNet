// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A tool that generates images using a model like gpt-image-1.
    /// </summary>
    public sealed class ImageGenerationTool : ITool
    {
        public static implicit operator Tool(ImageGenerationTool imageGenerationTool) => new(imageGenerationTool as ITool);

        public ImageGenerationTool() { }

        public ImageGenerationTool(
            Model model = null,
            string background = null,
            InputImageMask inputImageMask = null,
            string moderation = null,
            int? outputCompression = null,
            string outputFormat = null,
            int? partialImages = null,
            string quality = null,
            string size = null)
        {
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.GPT_Image_1 : model;
            Background = background;
            InputImageMask = inputImageMask;
            Moderation = moderation;
            OutputCompression = outputCompression;
            OutputFormat = outputFormat;
            PartialImages = partialImages;
            Quality = quality;
            Size = size;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "image_generation";

        /// <summary>
        /// Background type for the generated image. One of transparent, opaque, or auto. Default: auto.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("background")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Background { get; private set; }

        /// <summary>
        /// Optional mask for inpainting. Contains image_url (string, optional) and file_id (string, optional).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input_image_mask")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InputImageMask InputImageMask { get; private set; }

        /// <summary>
        /// The image generation model to use. Default: gpt-image-1.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("model")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Model { get; private set; }

        /// <summary>
        /// Moderation level for the generated image. Default: auto.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("moderation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Moderation { get; private set; }

        /// <summary>
        /// Compression level for the output image. Default: 100.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_compression")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OutputCompression { get; private set; }

        /// <summary>
        /// The output format of the generated image. One of png, webp, or jpeg. Default: png.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string OutputFormat { get; private set; }

        /// <summary>
        /// Number of partial images to generate in streaming mode, from 0 (default value) to 3.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("partial_images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PartialImages { get; private set; }

        /// <summary>
        /// The quality of the generated image. One of low, medium, high, or auto. Default: auto.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("quality")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Quality { get; private set; }

        /// <summary>
        /// The size of the generated image. One of 1024x1024, 1024x1536, 1536x1024, or auto. Default: auto.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Size { get; private set; }
    }
}
