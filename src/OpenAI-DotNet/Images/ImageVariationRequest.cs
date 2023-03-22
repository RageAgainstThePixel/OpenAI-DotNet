using System;
using System.IO;

namespace OpenAI.Images
{
    public sealed class ImageVariationRequest : IDisposable
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
        public ImageVariationRequest(string imagePath, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
            : this(File.OpenRead(imagePath), Path.GetFileName(imagePath), numberOfResults, size, user)
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
        public ImageVariationRequest(Stream image, string imageName, int numberOfResults, ImageSize size, string user)
        {
            Image = image;

            if (string.IsNullOrWhiteSpace(imageName))
            {
                imageName = "image.png";
            }

            ImageName = imageName;

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

        ~ImageVariationRequest() => Dispose(false);

        /// <summary>
        /// The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        public Stream Image { get; }

        public string ImageName { get; }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        public string User { get; }

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
