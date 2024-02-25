// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;
using System.IO;

namespace OpenAI.Images
{
    public sealed class ImageVariationRequest : AbstractBaseImageRequest, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageVariationRequest(
            string imagePath,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            Model model = null)
            : this(File.OpenRead(imagePath), Path.GetFileName(imagePath), numberOfResults, size, user, responseFormat, model)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// </param>
        /// <param name="imageName">
        /// The name of the image.
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageVariationRequest(
            Stream image,
            string imageName,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            Model model = null)
            : base(model, numberOfResults, size, responseFormat, user)
        {
            Image = image;

            if (string.IsNullOrWhiteSpace(imageName))
            {
                const string defaultImageName = "image.png";
                imageName = defaultImageName;
            }

            ImageName = imageName;

            if (numberOfResults is > 10 or < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            }
        }

        ~ImageVariationRequest() => Dispose(false);

        /// <summary>
        /// The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        public Stream Image { get; }

        public string ImageName { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
