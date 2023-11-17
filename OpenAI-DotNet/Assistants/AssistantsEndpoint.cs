using OpenAI.Extensions;
using OpenAI.Files;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants
{
    public class AssistantsEndpoint : BaseEndPoint
    {
        internal AssistantsEndpoint(OpenAIClient api) : base(api) { }

        protected override string Root => "assistants";

        /// <summary>
        /// Create an assistant with a model and instructions.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> CreateAssistantAsync(AssistantRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<AssistantResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> RetrieveAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<AssistantResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Modifies an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to modify.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> ModifyAssistantAsync(string assistantId, AssistantRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{assistantId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<AssistantResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the assistant was deleted.</returns>
        public async Task<bool> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.DeleteAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
        }

        /// <summary>
        /// Get list of assistants.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{Assistant}"/></returns>
        public async Task<ListResponse<AssistantResponse>> ListAssistantsAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<AssistantResponse>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Create an assistant file by attaching a File to an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant for which to create a File.
        /// </param>
        /// <param name="file">
        /// A <see cref="FileData"/> (with purpose="assistants") that the assistant should use.
        /// Useful for tools like retrieval and code_interpreter that can access files.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFile"/>.</returns>
        public async Task<AssistantFile> CreateAssistantFileAsync(string assistantId, FileData file, CancellationToken cancellationToken = default)
        {
            if (!file.Purpose.Equals("assistants"))
            {
                throw new InvalidOperationException($"{nameof(file)}.{nameof(file.Purpose)} must be 'assistants'!");
            }

            var jsonContent = JsonSerializer.Serialize(new { file_id = file.Id }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{assistantId}/files"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<AssistantFile>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves an AssistantFile.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant who the file belongs to.</param>
        /// <param name="fileId">The ID of the file we're getting.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFile"/>.</returns>
        public async Task<AssistantFile> RetrieveAssistantFileAsync(string assistantId, string fileId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<AssistantFile>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete an assistant file.
        /// </summary>
        /// <param name="file"><see cref="AssistantFile"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was deleted.</returns>
        public async Task<bool> DeleteAssistantFileAsync(AssistantFile file, CancellationToken cancellationToken = default)
            => await DeleteAssistantFileAsync(file.AssistantId, file.Id, cancellationToken);

        /// <summary>
        /// Delete an assistant file.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant that the file belongs to.</param>
        /// <param name="fileId">The ID of the file to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was deleted.</returns>
        public async Task<bool> DeleteAssistantFileAsync(string assistantId, string fileId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.DeleteAsync(GetUrl($"/{assistantId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
        }

        /// <summary>
        /// Returns a list of assistant files.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant the file belongs to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{AssistantFile}"/>.</returns>
        public async Task<ListResponse<AssistantFile>> ListAssistantFilesAsync(string assistantId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}/files", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<AssistantFile>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }
    }
}