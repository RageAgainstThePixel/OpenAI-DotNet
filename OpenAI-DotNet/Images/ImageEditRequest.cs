using System;
using System.IO;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    public sealed class ImageEditRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="maskPath">
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </param>
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
        public ImageEditRequest(string imagePath, string maskPath, string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Could not find the {nameof(imagePath)} file located at {imagePath}");
            }

            Image = File.OpenRead(imagePath);
            ImageName = Path.GetFileName(imagePath);

            if (!File.Exists(maskPath))
            {
                throw new FileNotFoundException($"Could not find the {nameof(maskPath)} file located at {maskPath}");
            }

            Mask = File.OpenRead(maskPath);
            MaskName = Path.GetFileName(maskPath);

            if (prompt.Length > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            }

            Prompt = prompt;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="imageName">Name of the image file.</param>
        /// <param name="mask">
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </param>
        /// <param name="maskName">Name of the mask file.</param>
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
        public ImageEditRequest(Stream image, string imageName, Stream mask, string maskName, string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
        {
            Image = image;
            ImageName = imageName;
            Mask = mask;
            MaskName = maskName;

            if (prompt.Length > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            }

            Prompt = prompt;

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

        ~ImageEditRequest()
        {
            Image?.Close();
            Image?.Dispose();
            Mask?.Close();
            Mask?.Dispose();
        }

        /// <summary>
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </summary>
        [JsonIgnore]
        public Stream Image { get; }

        [JsonIgnore]
        public string ImageName { get; }

        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        [JsonIgnore]
        public Stream Mask { get; }

        [JsonIgnore]
        public string MaskName { get; }

        /// <summary>
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; }

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