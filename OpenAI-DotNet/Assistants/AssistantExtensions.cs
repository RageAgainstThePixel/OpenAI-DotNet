using System.Threading;
using System.Threading.Tasks;
using OpenAI.Files;

namespace OpenAI.Assistants
{
    public static class AssistantExtensions
    {
        /// <summary>
        /// Modify the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="request"><see cref="CreateAssistantRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public static async Task<AssistantResponse> ModifyAsync(this AssistantResponse assistant, CreateAssistantRequest request, CancellationToken cancellationToken = default)
        {
            request = new CreateAssistantRequest(assistant: assistant, model: request.Model, name: request.Name, description: request.Description, instructions: request.Instructions, tools: request.Tools, files: request.FileIds, metadata: request.Metadata);
            return await assistant.Client.AssistantsEndpoint.ModifyAssistantAsync(assistantId: assistant.Id, request: request, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Delete the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the <see cref="assistant"/> was successfully deleted.</returns>
        public static async Task<bool> DeleteAsync(this AssistantResponse assistant, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.DeleteAssistantAsync(assistant.Id, cancellationToken).ConfigureAwait(false);

        #region Files

        /// <summary>
        /// Returns a list of assistant files.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{AssistantFile}"/>.</returns>
        public static async Task<ListResponse<AssistantFileResponse>> ListFilesAsync(this AssistantResponse assistant, ListQuery query = null, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.ListAssistantFilesAsync(assistant.Id, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Attach a file to the  <see cref="assistant"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="file">
        /// A <see cref="FileResponse"/> (with purpose="assistants") that the assistant should use.
        /// Useful for tools like retrieval and code_interpreter that can access files.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFileResponse"/>.</returns>
        public static async Task<AssistantFileResponse> AttachFileAsync(this AssistantResponse assistant, FileResponse file, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.AttachAssistantFileAsync(assistant.Id, file, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Uploads a new file at the specified <see cref="filePath"/> and attaches it to the <see cref="assistant"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="filePath">The local file path to upload.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFileResponse"/>.</returns>
        public static async Task<AssistantFileResponse> UploadFileAsync(this AssistantResponse assistant, string filePath, CancellationToken cancellationToken = default)
        {
            var file = await assistant.Client.FilesEndpoint.UploadFileAsync(new FileUploadRequest(filePath, "assistant"), cancellationToken).ConfigureAwait(false);
            return await assistant.AttachFileAsync(file, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the <see cref="AssistantFileResponse"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="fileId">The ID of the file we're getting.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFileResponse"/>.</returns>
        public static async Task<AssistantFileResponse> RetrieveFileAsync(this AssistantResponse assistant, string fileId, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.RetrieveAssistantFileAsync(assistant.Id, fileId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Downloads the <see cref="assistantFile"/> to the specified <see cref="directory"/>.
        /// </summary>
        /// <param name="assistantFile"><see cref="AssistantFileResponse"/>.</param>
        /// <param name="directory">The directory to download the file into.</param>
        /// <param name="deleteCachedFile">Optional, delete the cached file. Defaults to false.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The full path of the downloaded file.</returns>
        public static async Task<string> DownloadFileAsync(this AssistantFileResponse assistantFile, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
            => await assistantFile.Client.FilesEndpoint.DownloadFileAsync(assistantFile.Id, directory, deleteCachedFile, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Remove <see cref="assistant"/> file.
        /// </summary>
        /// <remarks>
        /// Note that removing an AssistantFile does not delete the original File object,
        /// it simply removes the association between that File and the Assistant.
        /// To delete a File, use <see cref="DeleteFileAsync"/>.
        /// </remarks>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="fileId">The ID of the file to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was removed.</returns>
        public static async Task<bool> RemoveFileAsync(this AssistantResponse assistant, string fileId, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.RemoveAssistantFileAsync(assistant.Id, fileId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Removes and Deletes a file from the <see cref="assistant"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="fileId">The ID of the file to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the file was successfully removed from the assistant and deleted.</returns>
        public static async Task<bool> DeleteFileAsync(this AssistantResponse assistant, string fileId, CancellationToken cancellationToken = default)
        {
            var isRemoved = await assistant.RemoveFileAsync(fileId, cancellationToken).ConfigureAwait(false);
            return isRemoved && await assistant.Client.FilesEndpoint.DeleteFileAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        #endregion Files
    }
}