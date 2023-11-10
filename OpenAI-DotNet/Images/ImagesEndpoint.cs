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
    public sealed class ImagesEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        internal ImagesEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "images";

        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
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
        /// The format in which the generated images are returned. Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optional, <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>A list of generated texture urls to download.</returns>
        [Obsolete]
        public async Task<IReadOnlyList<string>> GenerateImageAsync(
            string prompt,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            string user = null,
            ResponseFormat responseFormat = ResponseFormat.Url,
            CancellationToken cancellationToken = default)
            => await GenerateImageAsync(new ImageGenerationRequest(prompt, numberOfResults, size, user, responseFormat), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="request"><see cref="ImageGenerationRequest"/></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IReadOnlyList<string>> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var endpoint = GetUrl($"/generations{(Api.OpenAIClientSettings.IsAzureDeployment ? ":submit" : string.Empty)}");
            var response = await Api.Client.PostAsync(endpoint, jsonContent, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an edited or extended image given an original image and a prompt.
        /// </summary>
        /// <param name="image">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="mask">
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned. Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        [Obsolete]
        public async Task<IReadOnlyList<string>> CreateImageEditAsync(
            string image,
            string mask,
            string prompt,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            ResponseFormat responseFormat = ResponseFormat.Url,
            string user = null,
            CancellationToken cancellationToken = default)
            => await CreateImageEditAsync(new ImageEditRequest(image, mask, prompt, numberOfResults, size, user, responseFormat), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates an edited or extended image given an original image and a prompt.
        /// </summary>
        /// <param name="request"><see cref="ImageEditRequest"/></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IReadOnlyList<string>> CreateImageEditAsync(ImageEditRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent();
            using var imageData = new MemoryStream();
            await request.Image.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(imageData.ToArray()), "image", request.ImageName);

            if (request.Mask != null)
            {
                using var maskData = new MemoryStream();
                await request.Mask.CopyToAsync(maskData, cancellationToken).ConfigureAwait(false);
                content.Add(new ByteArrayContent(maskData.ToArray()), "mask", request.MaskName);
            }

            content.Add(new StringContent(request.Prompt), "prompt");
            content.Add(new StringContent(request.Number.ToString()), "n");
            content.Add(new StringContent(request.Size), "size");
            content.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

            if (!string.IsNullOrWhiteSpace(request.User))
            {
                content.Add(new StringContent(request.User), "user");
            }

            request.Dispose();
            var response = await Api.Client.PostAsync(GetUrl("/edits"), content, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a variation of a given image.
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
        /// <param name="responseFormat">
        /// The format in which the generated images are returned. Must be one of url or b64_json.
        /// <para/> Defaults to <see cref="ResponseFormat.Url"/>
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        [Obsolete]
        public async Task<IReadOnlyList<string>> CreateImageVariationAsync(
            string imagePath,
            int numberOfResults = 1,
            ImageSize size = ImageSize.Large,
            ResponseFormat responseFormat = ResponseFormat.Url,
            string user = null,
            CancellationToken cancellationToken = default)
            => await CreateImageVariationAsync(new ImageVariationRequest(imagePath, numberOfResults, size, user, responseFormat), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="request"><see cref="ImageVariationRequest"/></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IReadOnlyList<string>> CreateImageVariationAsync(ImageVariationRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent();
            using var imageData = new MemoryStream();
            await request.Image.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(imageData.ToArray()), "image", request.ImageName);
            content.Add(new StringContent(request.Number.ToString()), "n");
            content.Add(new StringContent(request.Size), "size");
            content.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

            if (!string.IsNullOrWhiteSpace(request.User))
            {
                content.Add(new StringContent(request.User), "user");
            }

            request.Dispose();
            var response = await Api.Client.PostAsync(GetUrl("/variations"), content, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<string>> DeserializeResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var resultAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var imagesResponse = response.DeserializeResponse<ImagesResponse>(resultAsString, OpenAIClient.JsonSerializationOptions);

            if (imagesResponse?.Results is not { Count: not 0 })
            {
                throw new HttpRequestException($"{nameof(DeserializeResponseAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {resultAsString}");
            }

            return imagesResponse.Results.Select(imageResult => string.IsNullOrWhiteSpace(imageResult.Url) ? imageResult.B64_Json : imageResult.Url).ToList();
        }
    }
}
