// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;

namespace OpenAI.Images
{
    /// <summary>
    /// Abstract base image class for making requests.
    /// </summary>
    public abstract class AbstractBaseImageRequest
    {
        protected AbstractBaseImageRequest(Model model = null, int? numberOfResults = null, string size = null, ImageResponseFormat responseFormat = 0, string user = null)
        {
            Model = model?.Id;
            Number = numberOfResults;
            Size = size;
            User = user;
            ResponseFormat = responseFormat;
        }

        [Obsolete("Use new .ctor overload")]
        protected AbstractBaseImageRequest(Model model, int numberOfResults, ImageSize size, ImageResponseFormat responseFormat, string user)
        {
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.DallE_2 : model;
            Number = numberOfResults;
            Size = size switch
            {
                ImageSize.Small => "256x256",
                ImageSize.Medium => "512x512",
                ImageSize.Large => "1024x1024",
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };
            User = user;
            ResponseFormat = responseFormat;
        }

        /// <summary>
        /// The model to use for image generation.
        /// Only `dall-e-2` and `gpt-image-1` are supported.
        /// Defaults to `dall-e-2` unless a parameter specific to `gpt-image-1` is used.
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        public int? Number { get; private set; }

        /// <summary>
        /// The format in which generated images with `dall-e-2` and `dall-e-3`
        /// are returned. Must be one of `url` or `b64_json`. URLs are only
        /// valid for 60 minutes after the image has been generated.
        /// `gpt-image-1` does not support urls and only supports base64-encoded images.
        /// </summary>
        public ImageResponseFormat ResponseFormat { get; private set; }

        /// <summary>
        /// The size of the generated images. Must be one of `1024x1024`, `1536x1024` (landscape), `1024x1536` (portrait), or `auto` (default value) for `gpt-image-1`, and one of `256x256`, `512x512`, or `1024x1024` for `dall-e-2`.
        /// </summary>
        public string Size { get; private set; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        public string User { get; private set; }
    }
}
