using System;
using System.IO;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    public sealed class ImageVariationRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
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
        public ImageVariationRequest(string imagePath, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Could not find the {nameof(imagePath)} file located at {imagePath}");
            }

            Image = File.OpenRead(imagePath);
            ImageName = Path.GetFileName(imagePath);

            if (numberOfResults is > 10 or < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            }

            Number = numberOfResults;

            Size = size switch
            {
                ImageSize.Small => "256x256",
                ImageSize.Medium => "512x512",
                ImageSize.Large => "1024x1024",
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };

            User = user;
        }

        ~ImageVariationRequest()
        {
            Image?.Close();
            Image?.Dispose();
        }

        /// <summary>
        /// The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        [JsonIgnore]
        public Stream Image { get; }

        [JsonIgnore]
        public string ImageName { get; }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        [JsonPropertyName("n")]
        public int Number { get; }

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