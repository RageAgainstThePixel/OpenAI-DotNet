﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Files
{
    /// <summary>
    /// Files are used to upload documents that can be used with features like Fine-tuning.
    /// <see href="https://beta.openai.com/docs/api-reference/fine-tunes"/>
    /// </summary>
    public class FilesEndpoint : BaseEndPoint
    {
        private class FilesList
        {
            [JsonPropertyName("data")]
            public List<FileData> Data { get; set; }
        }

        private class FileDeleteResponse
        {
            [JsonPropertyName("deleted")]
            public bool Deleted { get; set; }
        }

        /// <inheritdoc />
        public FilesEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "files";

        /// <summary>
        /// Returns a list of files that belong to the user's organization.
        /// </summary>
        /// <returns>List of <see cref="FileData"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IReadOnlyList<FileData>> ListFilesAsync()
        {
            var response = await Api.Client.GetAsync(GetUrl()).ConfigureAwait(false);
            var resultAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<FilesList>(resultAsString, Api.JsonSerializationOptions)?.Data;
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
        /// <returns><see cref="FileData"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FileData> UploadFileAsync(string filePath, string purpose, CancellationToken cancellationToken = default)
            => await UploadFileAsync(new FileUploadRequest(filePath, purpose), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features.
        /// Currently, the size of all the files uploaded by one organization can be up to 1 GB.
        /// Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="request"><see cref="FileUploadRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FileData"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FileData> UploadFileAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
        {
            using var fileData = new MemoryStream();
            using var content = new MultipartFormDataContent();
            await request.File.CopyToAsync(fileData, cancellationToken).ConfigureAwait(false);
            content.Add(new StringContent(request.Purpose), "purpose");
            content.Add(new ByteArrayContent(fileData.ToArray()), "file", request.FileName);
            request.Dispose();

            var response = await Api.Client.PostAsync(GetUrl(), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FileData>(responseAsString, Api.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was successfully deleted.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<bool> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            return await InternalDeleteFileAsync(1).ConfigureAwait(false);

            async Task<bool> InternalDeleteFileAsync(int attempt)
            {
                var response = await Api.Client.DeleteAsync(GetUrl($"/{fileId}"), cancellationToken).ConfigureAwait(false);
                // We specifically don't use the extension method here bc we need to check if it's still processing the file.
                var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    if (responseAsString.Contains("File is still processing. Check back later."))
                    {
                        // back off requests on each attempt
                        await Task.Delay(1000 * attempt, cancellationToken).ConfigureAwait(false);
                        return await InternalDeleteFileAsync(attempt + 1).ConfigureAwait(false);
                    }

                    throw new HttpRequestException($"{nameof(DeleteFileAsync)} Failed!  HTTP status code: {response.StatusCode}. Response: {responseAsString}");
                }

                return JsonSerializer.Deserialize<FileDeleteResponse>(responseAsString, Api.JsonSerializationOptions)?.Deleted ?? false;
            }
        }

        /// <summary>
        /// Returns information about a specific file.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FileData> GetFileInfoAsync(string fileId)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{fileId}")).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<FileData>(responseAsString, Api.JsonSerializationOptions);
        }

        /// <summary>
        /// Downloads the specified file.
        /// </summary>
        /// <param name="fileId">The file id to download.</param>
        /// <param name="directory">The directory to download the file into.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<string> DownloadFileAsync(string fileId, string directory, CancellationToken cancellationToken = default)
        {
            var fileData = await GetFileInfoAsync(fileId).ConfigureAwait(false);
            return await DownloadFileAsync(fileData, directory, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads the specified file.
        /// </summary>
        /// <param name="fileData"><see cref="FileData"/> to download.</param>
        /// <param name="directory">The directory to download the file into.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/></param>
        /// <returns>The full path of the downloaded file.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> DownloadFileAsync(FileData fileData, string directory, CancellationToken cancellationToken = default)
        {
            await using var response = await Api.Client.GetStreamAsync(GetUrl($"/{fileData.Id}/content"), cancellationToken).ConfigureAwait(false);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory ?? throw new ArgumentNullException(nameof(directory)));
            }

            var filePath = Path.Combine(directory, fileData.FileName);
            await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            await response.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            return filePath;
        }
    }
}
