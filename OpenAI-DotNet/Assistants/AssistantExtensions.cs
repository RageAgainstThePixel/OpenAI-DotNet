// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Files;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Create a thread and run it.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="request">Optional, <see cref="CreateThreadRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateThreadAndRunAsync(this AssistantResponse assistant, CreateThreadRequest request = null, CancellationToken cancellationToken = default)
            => await assistant.Client.ThreadsEndpoint.CreateThreadAndRunAsync(new CreateThreadAndRunRequest(assistant.Id, createThreadRequest: request), cancellationToken).ConfigureAwait(false);

        #region Files

        /// <summary>
        /// Returns a list of assistant files.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{AssistantFile}"/>.</returns>
        public static async Task<ListResponse<AssistantFileResponse>> ListFilesAsync(this AssistantResponse assistant, ListQuery query = null, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.ListFilesAsync(assistant.Id, query, cancellationToken).ConfigureAwait(false);

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
            => await assistant.Client.AssistantsEndpoint.AttachFileAsync(assistant.Id, file, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Uploads a new file at the specified <see cref="filePath"/> and attaches it to the <see cref="assistant"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="filePath">The local file path to upload.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFileResponse"/>.</returns>
        public static async Task<AssistantFileResponse> UploadFileAsync(this AssistantResponse assistant, string filePath, CancellationToken cancellationToken = default)
        {
            var file = await assistant.Client.FilesEndpoint.UploadFileAsync(new FileUploadRequest(filePath, "assistants"), cancellationToken).ConfigureAwait(false);
            return await assistant.AttachFileAsync(file, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads a new file at the specified path and attaches it to the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="stream">The file contents to upload.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantFileResponse"/>.</returns>

        public static async Task<AssistantFileResponse> UploadFileAsync(this AssistantResponse assistant, Stream stream, string fileName, CancellationToken cancellationToken = default)
        {
            var file = await assistant.Client.FilesEndpoint.UploadFileAsync(new FileUploadRequest(stream, fileName, "assistants"), cancellationToken).ConfigureAwait(false);
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
            => await assistant.Client.AssistantsEndpoint.RetrieveFileAsync(assistant.Id, fileId, cancellationToken).ConfigureAwait(false);

        // TODO 400 bad request errors. Likely OpenAI bug downloading assistant file content.
        ///// <summary>
        ///// Downloads the <see cref="assistantFile"/> to the specified <see cref="directory"/>.
        ///// </summary>
        ///// <param name="assistantFile"><see cref="AssistantFileResponse"/>.</param>
        ///// <param name="directory">The directory to download the file into.</param>
        ///// <param name="deleteCachedFile">Optional, delete the cached file. Defaults to false.</param>
        ///// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        ///// <returns>The full path of the downloaded file.</returns>
        //public static async Task<string> DownloadFileAsync(this AssistantFileResponse assistantFile, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
        //    => await assistantFile.Client.FilesEndpoint.DownloadFileAsync(assistantFile.Id, directory, deleteCachedFile, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Remove the file from the assistant it is attached to.
        /// </summary>
        /// <remarks>
        /// Note that removing an AssistantFile does not delete the original File object,
        /// it simply removes the association between that File and the Assistant.
        /// To delete a File, use <see cref="DeleteFileAsync(AssistantFileResponse,CancellationToken)"/>.
        /// </remarks>
        /// <param name="file"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was removed.</returns>
        public static async Task<bool> RemoveFileAsync(this AssistantFileResponse file, CancellationToken cancellationToken = default)
            => await file.Client.AssistantsEndpoint.RemoveFileAsync(file.AssistantId, file.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Remove the file from the assistant it is attached to.
        /// </summary>
        /// <remarks>
        /// Note that removing an AssistantFile does not delete the original File object,
        /// it simply removes the association between that File and the Assistant.
        /// To delete a File, use <see cref="DeleteFileAsync(AssistantFileResponse,CancellationToken)"/>.
        /// </remarks>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="fileId">The ID of the file to remove.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if file was removed.</returns>
        public static async Task<bool> RemoveFileAsync(this AssistantResponse assistant, string fileId, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.RemoveFileAsync(assistant.Id, fileId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Removes and Deletes a file from the assistant.
        /// </summary>
        /// <param name="file"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the file was successfully removed from the assistant and deleted.</returns>
        public static async Task<bool> DeleteFileAsync(this AssistantFileResponse file, CancellationToken cancellationToken = default)
        {
            var isRemoved = await file.RemoveFileAsync(cancellationToken).ConfigureAwait(false);
            return isRemoved && await file.Client.FilesEndpoint.DeleteFileAsync(file.Id, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes and Deletes a file from the <see cref="assistant"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="fileId">The ID of the file to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the file was successfully removed from the assistant and deleted.</returns>
        public static async Task<bool> DeleteFileAsync(this AssistantResponse assistant, string fileId, CancellationToken cancellationToken = default)
        {
            var isRemoved = await assistant.Client.AssistantsEndpoint.RemoveFileAsync(assistant.Id, fileId, cancellationToken).ConfigureAwait(false);
            if (!isRemoved) { return false; }
            return await assistant.Client.FilesEndpoint.DeleteFileAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        #endregion Files

        #region Tools

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <returns>Tool output result as <see cref="string"/></returns>
        public static string InvokeToolCall(this AssistantResponse assistant, ToolCall toolCall)
        {
            if (toolCall.Type != "function")
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.Type == "function" && tool.Function.Name == toolCall.FunctionCall.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Type}");
            tool.Function.Arguments = toolCall.FunctionCall.Arguments;
            return tool.InvokeFunction();
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tool output result as <see cref="string"/></returns>
        public static async Task<string> InvokeToolCallAsync(this AssistantResponse assistant, ToolCall toolCall, CancellationToken cancellationToken = default)
        {
            if (toolCall.Type != "function")
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.Type == "function" && tool.Function.Name == toolCall.FunctionCall.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Type}");
            tool.Function.Arguments = toolCall.FunctionCall.Arguments;
            return await tool.InvokeFunctionAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls the tool's function, with the provided arguments from the toolCall and returns the output.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <returns><see cref="ToolOutput"/>.</returns>
        public static ToolOutput GetToolOutput(this AssistantResponse assistant, ToolCall toolCall)
            => new(toolCall.Id, assistant.InvokeToolCall(toolCall));

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCalls">A collection of <see cref="ToolCall"/>s.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static IReadOnlyList<ToolOutput> GetToolOutputs(this AssistantResponse assistant, IEnumerable<ToolCall> toolCalls)
            => toolCalls.Select(assistant.GetToolOutput).ToList();

        /// <summary>
        /// Calls the tool's function, with the provided arguments from the toolCall and returns the output.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ToolOutput"/>.</returns>
        public static async Task<ToolOutput> GetToolOutputAsync(this AssistantResponse assistant, ToolCall toolCall, CancellationToken cancellationToken = default)
        {
            var output = await assistant.InvokeToolCallAsync(toolCall, cancellationToken).ConfigureAwait(false);
            return new ToolOutput(toolCall.Id, output);
        }

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCalls">A collection of <see cref="ToolCall"/>s.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static async Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, IEnumerable<ToolCall> toolCalls, CancellationToken cancellationToken = default)
            => await Task.WhenAll(toolCalls.Select(async toolCall => await assistant.GetToolOutputAsync(toolCall, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);

        #endregion Tools
    }
}
