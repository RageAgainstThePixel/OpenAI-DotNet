// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Threads;
using System;
using System.Collections.Generic;
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
            => await assistant.Client.AssistantsEndpoint.ModifyAssistantAsync(
                assistantId: assistant.Id,
                request: request ?? new CreateAssistantRequest(assistant),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Get the latest status of the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public static async Task<AssistantResponse> UpdateAsync(this AssistantResponse assistant, CancellationToken cancellationToken = default)
            => await assistant.Client.AssistantsEndpoint.RetrieveAssistantAsync(assistant, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Delete the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="deleteToolResources">Optional, should tool resources, such as vector stores be deleted when this assistant is deleted?</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the <see cref="assistant"/> was successfully deleted.</returns>
        public static async Task<bool> DeleteAsync(this AssistantResponse assistant, bool deleteToolResources = false, CancellationToken cancellationToken = default)
        {
            if (deleteToolResources)
            {
                assistant = await assistant.UpdateAsync(cancellationToken).ConfigureAwait(false);
            }

            var deleteTasks = new List<Task<bool>> { assistant.Client.AssistantsEndpoint.DeleteAssistantAsync(assistant.Id, cancellationToken) };

            if (deleteToolResources && assistant.ToolResources?.FileSearch?.VectorStoreIds is { Count: > 0 })
            {
                deleteTasks.AddRange(
                    from vectorStoreId in assistant.ToolResources?.FileSearch?.VectorStoreIds
                    where !string.IsNullOrWhiteSpace(vectorStoreId)
                    select assistant.Client.VectorStoresEndpoint.DeleteVectorStoreAsync(vectorStoreId, cancellationToken));
            }

            await Task.WhenAll(deleteTasks).ConfigureAwait(false);
            return deleteTasks.TrueForAll(task => task.Result);
        }

        /// <summary>
        /// Create a thread and run it.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="request">Optional, <see cref="CreateThreadRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateThreadAndRunAsync(this AssistantResponse assistant, CreateThreadRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            var threadRunRequest = new CreateThreadAndRunRequest(
                assistant.Id,
                assistant.Model,
                assistant.Instructions,
                assistant.Tools,
                assistant.ToolResources,
                assistant.Metadata,
                assistant.Temperature,
                assistant.TopP,
                jsonSchema: assistant.ResponseFormatObject?.JsonSchema,
                responseFormat: assistant.ResponseFormat,
                createThreadRequest: request);
            return await assistant.Client.ThreadsEndpoint.CreateThreadAndRunAsync(threadRunRequest, streamEventHandler, cancellationToken).ConfigureAwait(false);
        }

        #region Tools

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="ToolCall"/> if you know the method is synchronous.</remarks>
        public static string InvokeToolCall(this AssistantResponse assistant, ToolCall toolCall)
        {
            if (!toolCall.IsFunction)
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Function.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Function.Name}");
            return tool.InvokeFunction(toolCall);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <typeparam name="T">The expected signature return type.</typeparam>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="ToolCall"/> if you know the method is synchronous.</remarks>
        public static T InvokeToolCall<T>(this AssistantResponse assistant, ToolCall toolCall)
        {
            if (!toolCall.IsFunction)
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Function.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Function.Name}");
            return tool.InvokeFunction<T>(toolCall);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        public static async Task<string> InvokeToolCallAsync(this AssistantResponse assistant, ToolCall toolCall, CancellationToken cancellationToken = default)
        {
            if (!toolCall.IsFunction)
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.Type == "function" && tool.Function.Name == toolCall.Function.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Function.Name}");
            return await tool.InvokeFunctionAsync(toolCall, cancellationToken);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="ToolCall"/>.
        /// </summary>
        /// <typeparam name="T">The expected signature return type.</typeparam>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        public static async Task<T> InvokeToolCallAsync<T>(this AssistantResponse assistant, ToolCall toolCall, CancellationToken cancellationToken = default)
        {
            if (!toolCall.IsFunction)
            {
                throw new InvalidOperationException($"Cannot invoke built in tool {toolCall.Type}");
            }

            var tool = assistant.Tools.FirstOrDefault(tool => tool.Type == "function" && tool.Function.Name == toolCall.Function.Name) ??
                       throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.Id}] {toolCall.Function.Name}");
            return await tool.InvokeFunctionAsync<T>(toolCall, cancellationToken);
        }

        /// <summary>
        /// Calls the tool's function, with the provided arguments from the toolCall and returns the output.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="ToolCall"/>.</param>
        /// <returns><see cref="ToolOutput"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="ToolCall"/> if you know the method is synchronous.</remarks>
        public static ToolOutput GetToolOutput(this AssistantResponse assistant, ToolCall toolCall)
            => new(toolCall.Id, assistant.InvokeToolCall(toolCall));

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
        public static async Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, IEnumerable<Threads.ToolCall> toolCalls, CancellationToken cancellationToken = default)
            => await Task.WhenAll(toolCalls.Select(toolCall => assistant.GetToolOutputAsync(toolCall, cancellationToken))).ConfigureAwait(false);

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCalls">A collection of <see cref="ToolCall"/>s.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static async Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, IEnumerable<ToolCall> toolCalls, CancellationToken cancellationToken = default)
            => await Task.WhenAll(toolCalls.Select(toolCall => assistant.GetToolOutputAsync(toolCall, cancellationToken))).ConfigureAwait(false);

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="run">The <see cref="RunResponse"/> to complete the tool calls for.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static async Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, RunResponse run, CancellationToken cancellationToken = default)
            => await GetToolOutputsAsync(assistant, run.RequiredAction.SubmitToolOutputs.ToolCalls, cancellationToken).ConfigureAwait(false);

        #endregion Tools
    }
}
