// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;
using System.IO;

namespace OpenAI.Images
{
    public sealed class ImageEditRequest : AbstractBaseImageRequest, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageEditRequest(
            string imagePath,
            string prompt,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            Model model = null)
            : this(imagePath, null, prompt, numberOfResults, size, user, responseFormat, model)
        {
        }

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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageEditRequest(
            string imagePath,
            string maskPath,
            string prompt,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            Model model = null)
            : this(
                File.OpenRead(imagePath),
                Path.GetFileName(imagePath),
                string.IsNullOrWhiteSpace(maskPath) ? null : File.OpenRead(maskPath),
                string.IsNullOrWhiteSpace(maskPath) ? null : Path.GetFileName(maskPath),
                prompt,
                numberOfResults,
                size,
                user,
                responseFormat,
                model)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="imageName">Name of the image file.</param>
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageEditRequest(
            Stream image,
            string imageName,
            string prompt,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            Model model = null)
            : this(image, imageName, null, null, prompt, numberOfResults, size, user, responseFormat, model)
        {
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned.
        /// Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="model">
        /// The model to use for image generation.
        /// </param>
        public ImageEditRequest(
            Stream image,
            string imageName,
            Stream mask,
            string maskName,
            string prompt,
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

            if (mask != null)
            {
                Mask = mask;

                if (string.IsNullOrWhiteSpace(maskName))
                {
                    const string defaultMaskName = "mask.png";
                    maskName = defaultMaskName;
                }

                MaskName = maskName;
            }

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

        ~ImageEditRequest() => Dispose(false);

        /// <summary>
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </summary>
        public Stream Image { get; }

        public string ImageName { get; }

        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        public Stream Mask { get; }

        public string MaskName { get; }

        /// <summary>
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </summary>
        public string Prompt { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
                Mask?.Dispose();
                Mask?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
