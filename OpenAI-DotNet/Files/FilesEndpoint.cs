// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Files
{
    /// <summary>
    /// Files are used to upload documents that can be used with features like Fine-tuning.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/files"/>
    /// </summary>
    public sealed class FilesEndpoint : OpenAIBaseEndpoint
    {
        private class FilesList
        {
            [JsonPropertyName("data")]
            public IReadOnlyList<FileResponse> Files { get; set; }
        }

        /// <inheritdoc />
        public FilesEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "files";

        /// <summary>
        /// Returns a list of files that belong to the user's organization.
        /// </summary>
        /// <param name="purpose">List files with a specific purpose.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of <see cref="FileResponse"/>.</returns>
        public async Task<IReadOnlyList<FileResponse>> ListFilesAsync(string purpose = null, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> query = null;

            if (!string.IsNullOrWhiteSpace(purpose))
            {
                query = new Dictionary<string, string> { { nameof(purpose), purpose } };
            }

            using var response = await client.Client.GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            var resultAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FilesList>(resultAsString, OpenAIClient.JsonSerializationOptions)?.Files;
        }

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
        /// <returns><see cref="FileResponse"/>.</returns>
        public async Task<FileResponse> UploadFileAsync(string filePath, string purpose, CancellationToken cancellationToken = default)
            => await UploadFileAsync(new FileUploadRequest(filePath, purpose), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features.
        /// Currently, the size of all the files uploaded by one organization can be up to 1 GB.
        /// Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="request"><see cref="FileUploadRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FileResponse"/>.</returns>
        public async Task<FileResponse> UploadFileAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
        {
            using var fileData = new MemoryStream();
            using var content = new MultipartFormDataContent();
            await request.File.CopyToAsync(fileData, cancellationToken).ConfigureAwait(false);
            content.Add(new StringContent(request.Purpose), "purpose");
            content.Add(new ByteArrayContent(fileData.ToArray()), "file", request.FileName);
            request.Dispose();
            using var response = await client.Client.PostAsync(GetUrl(), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, content, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FileResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was successfully deleted.</returns>
        public async Task<bool> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            return await InternalDeleteFileAsync(1).ConfigureAwait(false);

            async Task<bool> InternalDeleteFileAsync(int attempt)
            {
                using var response = await client.Client.DeleteAsync(GetUrl($"/{fileId}"), cancellationToken).ConfigureAwait(false);
                // We specifically don't use the extension method here bc we need to check if it's still processing the file.
                var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    const string fileProcessing = "File is still processing. Check back later.";

                    if (response.StatusCode == /* 409 */ HttpStatusCode.Conflict ||
                        !string.IsNullOrWhiteSpace(responseAsString) &&
                        responseAsString.Contains(fileProcessing))
                    {
                        // back off requests on each attempt
                        await Task.Delay(1000 * attempt++, cancellationToken).ConfigureAwait(false);
                        return await InternalDeleteFileAsync(attempt).ConfigureAwait(false);
                    }
                }

                await response.CheckResponseAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
                return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
            }
        }

        /// <summary>
        /// Returns information about a specific file.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FileResponse"/></returns>
        public async Task<FileResponse> GetFileInfoAsync(string fileId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FileResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Downloads the specified file.
        /// </summary>
        /// <param name="fileId">The file id to download.</param>
        /// <param name="directory">The directory to download the file into.</param>
        /// <param name="deleteCachedFile">Optional, delete cached file. Default is false.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
        /// <returns>The full path of the downloaded file.</returns>
        public async Task<string> DownloadFileAsync(string fileId, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
        {
            var fileData = await GetFileInfoAsync(fileId, cancellationToken).ConfigureAwait(false);
            return await DownloadFileAsync(fileData, directory, deleteCachedFile, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads the specified file.
        /// </summary>
        /// <param name="fileData"><see cref="FileResponse"/> to download.</param>
        /// <param name="directory">The directory to download the file into.</param>
        /// <param name="deleteCachedFile">Optional, delete cached file. Default is false.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
        /// <returns>The full path of the downloaded file.</returns>
        public async Task<string> DownloadFileAsync(FileResponse fileData, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileData.FileName);

            if (File.Exists(filePath))
            {
                if (deleteCachedFile)
                {
                    File.Delete(filePath);
                }
                else
                {
                    return filePath;
                }
            }

            await using var response = await RetrieveFileStreamAsync(fileData, cancellationToken).ConfigureAwait(false);
            await using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            await response.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            return filePath;
        }

        /// <summary>
        /// Gets the specified file as stream
        /// </summary>
        /// <param name="fileData"><see cref="FileResponse"/> to download.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
        /// <returns>The file as a stream in an asynchronous operation.</returns>
        public async Task<Stream> RetrieveFileStreamAsync(FileResponse fileData, CancellationToken cancellationToken = default)
        {
            return await client.Client.GetStreamAsync(GetUrl($"/{fileData.Id}/content"), cancellationToken).ConfigureAwait(false);
        }
    }
}
