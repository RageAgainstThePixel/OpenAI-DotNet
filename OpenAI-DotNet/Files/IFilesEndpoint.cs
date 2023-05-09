using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Files;

public interface IFilesEndpoint
{
    /// <summary>
    /// Returns a list of files that belong to the user's organization.
    /// </summary>
    /// <returns>List of <see cref="FileData"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<FileData>> ListFilesAsync();

    /// <summary>
    /// Upload a file that contains document(s) to be used across various endpoints/features.
    /// Currently, the size of all the files uploaded by one organization can be up to 1 GB.
    /// Please contact us if you need to increase the storage limit.
    /// </summary>
    /// <param name="filePath">
    /// Local file path to upload.
    /// </param>
    /// <param name="purpose">
    /// The intended purpose of the uploaded documents.
    /// If the purpose is set to "fine-tune", each line is a JSON record with "prompt" and "completion"
    /// fields representing your training examples.
    /// </param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="FileData"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<FileData> UploadFileAsync(string filePath, string purpose, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a file that contains document(s) to be used across various endpoints/features.
    /// Currently, the size of all the files uploaded by one organization can be up to 1 GB.
    /// Please contact us if you need to increase the storage limit.
    /// </summary>
    /// <param name="request"><see cref="FileUploadRequest"/>.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="FileData"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<FileData> UploadFileAsync(FileUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file.
    /// </summary>
    /// <param name="fileId">The ID of the file to use for this request</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>True, if file was successfully deleted.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<bool> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns information about a specific file.
    /// </summary>
    /// <param name="fileId">The ID of the file to use for this request.</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<FileData> GetFileInfoAsync(string fileId);

    /// <summary>
    /// Downloads the specified file.
    /// </summary>
    /// <param name="fileId">The file id to download.</param>
    /// <param name="directory">The directory to download the file into.</param>
    /// <param name="deleteCachedFile">Optional, delete cached file. Default is false.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
    /// <returns>The full path of the downloaded file.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<string> DownloadFileAsync(string fileId, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the specified file.
    /// </summary>
    /// <param name="fileData"><see cref="FileData"/> to download.</param>
    /// <param name="directory">The directory to download the file into.</param>
    /// <param name="deleteCachedFile">Optional, delete cached file. Default is false.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
    /// <returns>The full path of the downloaded file.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<string> DownloadFileAsync(FileData fileData, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default);
}