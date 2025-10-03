// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Images
{
    /// <summary>
    /// Given a prompt and/or an input image, the model will generate a new image.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/images"/>
    /// </summary>
    public sealed class ImagesEndpoint : OpenAIBaseEndpoint
    {
        /// <inheritdoc />
        internal ImagesEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "images";

        protected override bool? IsAzureDeployment => true;

        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="request"><see cref="ImageGenerationRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        public async Task<IReadOnlyList<ImageResult>> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await PostAsync(GetUrl("/generations"), payload, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, payload, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an edited or extended image given an original image and a prompt.
        /// </summary>
        /// <param name="request"><see cref="ImageEditRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        public async Task<IReadOnlyList<ImageResult>> CreateImageEditAsync(ImageEditRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = new MultipartFormDataContent();

            try
            {
                if (!string.IsNullOrWhiteSpace(request.Model))
                {
                    payload.Add(new StringContent(request.Model), "model");
                }

                payload.Add(new StringContent(request.Prompt), "prompt");

                var imageLabel = request.Images.Count > 1 ? "image[]" : "image";

                async Task ProcessImageAsync(MultipartFormDataContent content, KeyValuePair<string, Stream> value)
                {
                    using var imageData = new MemoryStream();
                    var (name, image) = value;
                    await image.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
                    content.Add(new ByteArrayContent(imageData.ToArray()), imageLabel, name);
                }

                await Task.WhenAll(request.Images.Select(image => ProcessImageAsync(payload, image)).ToList());

                if (request.Mask != null)
                {
                    using var maskData = new MemoryStream();
                    await request.Mask.CopyToAsync(maskData, cancellationToken).ConfigureAwait(false);
                    payload.Add(new ByteArrayContent(maskData.ToArray()), "mask", request.MaskName);
                }

                if (request.Number.HasValue)
                {
                    payload.Add(new StringContent(request.Number.Value.ToString()), "n");
                }

                if (!string.IsNullOrWhiteSpace(request.Size))
                {
                    payload.Add(new StringContent(request.Size), "size");
                }

                if (!string.IsNullOrWhiteSpace(request.Quality))
                {
                    payload.Add(new StringContent(request.Quality), "quality");
                }

                if (request.ResponseFormat > 0)
                {
                    payload.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");
                }

                if (!string.IsNullOrWhiteSpace(request.User))
                {
                    payload.Add(new StringContent(request.User), "user");
                }
            }
            finally
            {
                request.Dispose();
            }

            using var response = await PostAsync(GetUrl("/edits"), payload, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, payload, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="request"><see cref="ImageVariationRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        public async Task<IReadOnlyList<ImageResult>> CreateImageVariationAsync(ImageVariationRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = new MultipartFormDataContent();

            try
            {
                using var imageData = new MemoryStream();
                await request.Image.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
                payload.Add(new ByteArrayContent(imageData.ToArray()), "image", request.ImageName);

                if (!string.IsNullOrWhiteSpace(request.Model))
                {
                    payload.Add(new StringContent(request.Model), "model");
                }

                if (request.Number.HasValue)
                {
                    payload.Add(new StringContent(request.Number.Value.ToString()), "n");
                }

                if (!string.IsNullOrWhiteSpace(request.Size))
                {
                    payload.Add(new StringContent(request.Size), "size");
                }

                if (request.ResponseFormat > 0)
                {
                    payload.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");
                }

                if (!string.IsNullOrWhiteSpace(request.User))
                {
                    payload.Add(new StringContent(request.User), "user");
                }
            }
            finally
            {
                request.Dispose();
            }

            using var response = await PostAsync(GetUrl("/variations"), payload, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, payload, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<ImageResult>> DeserializeResponseAsync(HttpResponseMessage response, HttpContent requestContent, CancellationToken cancellationToken = default)
        {
            var resultAsString = await response.ReadAsStringAsync(EnableDebug, requestContent, cancellationToken).ConfigureAwait(false);
            var imagesResponse = response.Deserialize<ImagesResponse>(resultAsString, client);

            if (imagesResponse == null ||
                imagesResponse.Results.Count == 0)
            {
                throw new HttpRequestException($"{nameof(DeserializeResponseAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {resultAsString}");
            }

            foreach (var result in imagesResponse.Results)
            {
                result.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(imagesResponse.CreatedAtUnixSeconds).UtcDateTime;
                result.Background = imagesResponse.Background;
                result.OutputFormat = imagesResponse.OutputFormat;
                result.Quality = imagesResponse.Quality;
                result.Size = imagesResponse.Size;
                result.Usage = imagesResponse.Usage;
            }

            return imagesResponse.Results;
        }
    }
}
