// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;
using System.IO;

namespace OpenAI.Images
{
    public sealed class ImageVariationRequest : AbstractBaseImageRequest, IDisposable
    {
        public ImageVariationRequest(
            string imagePath,
            int? numberOfResults = null,
            string size = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : this((Path.GetFileName(imagePath), File.OpenRead(imagePath)), numberOfResults, size, user, responseFormat, model)
        {
        }

        public ImageVariationRequest(
            (string, Stream) image,
            int? numberOfResults = null,
            string size = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : base(model, numberOfResults, size, responseFormat, user)
        {
            var (imageName, imageStream) = image;
            Image = imageStream ?? throw new ArgumentNullException(nameof(imageStream));
            ImageName = string.IsNullOrWhiteSpace(imageName) ? "image.png" : imageName;
        }

        #region Obsolete .ctors

        [Obsolete("Use new .ctor overload")]
        public ImageVariationRequest(
            string imagePath,
            int numberOfResults,
            ImageSize size,
            string user,
            ImageResponseFormat responseFormat,
            Model model)
            : this(File.OpenRead(imagePath), Path.GetFileName(imagePath), numberOfResults, size, user, responseFormat, model)
        {
        }

        [Obsolete("Use new .ctor overload")]
        public ImageVariationRequest(
            Stream image,
            string imageName,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ImageResponseFormat responseFormat = 0,
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

        #endregion Obsolete .ctors

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
