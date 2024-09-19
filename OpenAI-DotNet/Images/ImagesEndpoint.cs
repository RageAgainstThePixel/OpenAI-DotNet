// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.IO;
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
            using var response = await client.Client.PostAsync(GetUrl("/generations"), payload, cancellationToken).ConfigureAwait(false);
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
                using var imageData = new MemoryStream();
                await request.Image.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
                payload.Add(new ByteArrayContent(imageData.ToArray()), "image", request.ImageName);

                if (request.Mask != null)
                {
                    using var maskData = new MemoryStream();
                    await request.Mask.CopyToAsync(maskData, cancellationToken).ConfigureAwait(false);
                    payload.Add(new ByteArrayContent(maskData.ToArray()), "mask", request.MaskName);
                }

                payload.Add(new StringContent(request.Prompt), "prompt");
                payload.Add(new StringContent(request.Number.ToString()), "n");
                payload.Add(new StringContent(request.Size), "size");
                payload.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

                if (!string.IsNullOrWhiteSpace(request.User))
                {
                    payload.Add(new StringContent(request.User), "user");
                }
            }
            finally
            {
                request.Dispose();
            }

            using var response = await client.Client.PostAsync(GetUrl("/edits"), payload, cancellationToken).ConfigureAwait(false);
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
                payload.Add(new StringContent(request.Number.ToString()), "n");
                payload.Add(new StringContent(request.Size), "size");
                payload.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

                if (!string.IsNullOrWhiteSpace(request.User))
                {
                    payload.Add(new StringContent(request.User), "user");
                }
            }
            finally
            {
                request.Dispose();
            }

            using var response = await client.Client.PostAsync(GetUrl("/variations"), payload, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, payload, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<ImageResult>> DeserializeResponseAsync(HttpResponseMessage response, HttpContent requestContent, CancellationToken cancellationToken = default)
        {
            var resultAsString = await response.ReadAsStringAsync(EnableDebug, requestContent, cancellationToken).ConfigureAwait(false);
            var imagesResponse = response.Deserialize<ImagesResponse>(resultAsString, client);

            if (imagesResponse?.Results is not { Count: not 0 })
            {
                throw new HttpRequestException($"{nameof(DeserializeResponseAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {resultAsString}");
            }

            return imagesResponse.Results;
        }
    }
}
