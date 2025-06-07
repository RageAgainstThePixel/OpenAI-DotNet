// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace OpenAI.Images
{
    public sealed class ImageEditRequest : AbstractBaseImageRequest, IDisposable
    {
        public ImageEditRequest(
            string prompt,
            string imagePath,
            string maskPath = null,
            int? numberOfResults = null,
            string size = null,
            string quality = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
        : this(
            prompt: prompt,
            images: new Dictionary<string, Stream>
            {
                [Path.GetFileName(imagePath)] = File.OpenRead(imagePath)
            },
            mask: string.IsNullOrWhiteSpace(maskPath) ? default : (Path.GetFileName(maskPath), File.OpenRead(maskPath)),
            numberOfResults: numberOfResults,
            size: size,
            quality: quality,
            user: user,
            responseFormat: responseFormat,
            model: model)
        {
        }

        public ImageEditRequest(
            string prompt,
            (string, Stream) image,
            (string, Stream) mask = default,
            int? numberOfResults = null,
            string size = null,
            string quality = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : this(
                prompt: prompt,
                images: new Dictionary<string, Stream>
                {
                    [image.Item1] = image.Item2
                },
                mask: mask,
                numberOfResults: numberOfResults,
                size: size,
                quality: quality,
                user: user,
                responseFormat: responseFormat,
                model: model)
        {
        }

        public ImageEditRequest(
            string prompt,
            IReadOnlyDictionary<string, Stream> images,
            (string, Stream) mask = default,
            int? numberOfResults = null,
            string size = null,
            string quality = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : base(model, numberOfResults, size, responseFormat, user)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Images = images ?? throw new ArgumentNullException(nameof(images));

            if (Images.Count > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(images), "You can only provide up to 16 images.");
            }

            Quality = quality;

            if (mask != default)
            {
                MaskName = mask.Item1;
                Mask = mask.Item2;
            }
        }

        #region Obsolete .ctors

        [Obsolete("Use new .ctor overload")]
        public ImageEditRequest(
            string imagePath,
            string prompt,
            int numberOfResults,
            ImageSize size = ImageSize.Large,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : this(imagePath, null, prompt, numberOfResults, size, user, responseFormat, model)
        {
        }

        [Obsolete("Use new .ctor overload")]
        public ImageEditRequest(
            string imagePath,
            string maskPath,
            string prompt,
            int numberOfResults,
            ImageSize size = ImageSize.Large,
            string user = null,
            ImageResponseFormat responseFormat = 0,
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

        [Obsolete("Use new .ctor overload")]
        public ImageEditRequest(
            Stream image,
            string imageName,
            string prompt,
            int numberOfResults,
            ImageSize size = ImageSize.Large,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : this(image, imageName, null, null, prompt, numberOfResults, size, user, responseFormat, model)
        {
        }

        [Obsolete("Use new .ctor overload")]
        public ImageEditRequest(
            Stream image,
            string imageName,
            Stream mask,
            string maskName,
            string prompt,
            int numberOfResults,
            ImageSize size = ImageSize.Large,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : base(model, numberOfResults, size, responseFormat, user)
        {
            var images = new ConcurrentDictionary<string, Stream>();

            if (string.IsNullOrWhiteSpace(imageName))
            {
                const string defaultImageName = "image.png";
                imageName = defaultImageName;
            }

            images[imageName] = image;
            Images = images;

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

            Prompt = prompt;
        }

        #endregion Obsolete .ctors

        ~ImageEditRequest() => Dispose(false);

        /// <summary>
        /// A text description of the desired image(s).
        /// The maximum length is 1000 characters for `dall-e-2`, and 32000 characters for `gpt-image-1`.
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// The quality of the image that will be generated.
        /// `high`, `medium` and `low` are only supported for `gpt-image-1`.
        /// `dall-e-2` only supports `standard` quality. Defaults to `auto`.
        /// </summary>
        public string Quality { get; private set; }

        /// <summary>
        /// The image(s) to edit. 
        /// Must be a supported image file or an array of images.
        /// For `gpt-image-1`, each image should be a `png`, `webp`, or `jpg` file less than 25MB. 
        /// You can provide up to 16 images.
        /// For `dall-e-2`, you can only provide one image, and it should be a square `png` file less than 4MB.
        /// </summary>
        public IReadOnlyDictionary<string, Stream> Images { get; }

        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where `image` should be edited.
        /// If there are multiple images provided, the mask will be applied on the first image.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as `image`.
        /// </summary>
        public Stream Mask { get; }

        public string MaskName { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var image in Images.Values)
                {
                    image?.Dispose();
                }

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
