using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Images;

public interface IImagesEndpoint
{
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
    Task<IReadOnlyList<string>> GenerateImageAsync(
        string prompt,
        int numberOfResults = 1,
        ImageSize size = ImageSize.Large,
        string user = null,
        ResponseFormat responseFormat = ResponseFormat.Url,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    /// <param name="request"><see cref="ImageGenerationRequest"/></param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>A list of generated texture urls to download.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<string>> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default);

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
    Task<IReadOnlyList<string>> CreateImageEditAsync(
        string image,
        string mask,
        string prompt,
        int numberOfResults = 1,
        ImageSize size = ImageSize.Large,
        ResponseFormat responseFormat = ResponseFormat.Url,
        string user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an edited or extended image given an original image and a prompt.
    /// </summary>
    /// <param name="request"><see cref="ImageEditRequest"/></param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>A list of generated texture urls to download.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<string>> CreateImageEditAsync(ImageEditRequest request, CancellationToken cancellationToken = default);

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
    Task<IReadOnlyList<string>> CreateImageVariationAsync(
        string imagePath,
        int numberOfResults = 1,
        ImageSize size = ImageSize.Large,
        ResponseFormat responseFormat = ResponseFormat.Url,
        string user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a variation of a given image.
    /// </summary>
    /// <param name="request"><see cref="ImageVariationRequest"/></param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>A list of generated texture urls to download.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<string>> CreateImageVariationAsync(ImageVariationRequest request, CancellationToken cancellationToken = default);
}