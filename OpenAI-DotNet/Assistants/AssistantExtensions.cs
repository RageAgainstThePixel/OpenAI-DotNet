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
        public static Task<AssistantResponse> ModifyAsync(this AssistantResponse assistant, CreateAssistantRequest request, CancellationToken cancellationToken = default)
            => assistant.Client.AssistantsEndpoint.ModifyAssistantAsync(
                assistantId: assistant.Id,
                request: request ?? new CreateAssistantRequest(assistant),
                cancellationToken: cancellationToken);

        /// <summary>
        /// Get the latest status of the assistant.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public static Task<AssistantResponse> UpdateAsync(this AssistantResponse assistant, CancellationToken cancellationToken = default)
            => assistant.Client.AssistantsEndpoint.RetrieveAssistantAsync(assistant, cancellationToken);

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
        public static Task<RunResponse> CreateThreadAndRunAsync(this AssistantResponse assistant, CreateThreadRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            var threadRunRequest = new CreateThreadAndRunRequest(
                assistant.Id,
                assistant.Model,
                assistant.Instructions,
                assistant.Tools,
                assistant.ToolResources,
                assistant.Metadata,
                assistant.ReasoningEffort > 0 ? null : assistant.Temperature,
                assistant.ReasoningEffort > 0 ? null : assistant.TopP,
                jsonSchema: assistant.ResponseFormatObject?.JsonSchema,
                responseFormat: assistant.ResponseFormat,
                createThreadRequest: request);
            return assistant.Client.ThreadsEndpoint.CreateThreadAndRunAsync(threadRunRequest, streamEventHandler, cancellationToken);
        }

        #region Tools

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="IToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="IToolCall"/> if you know the method is synchronous.</remarks>
        public static string InvokeToolCall(this AssistantResponse assistant, IToolCall toolCall)
        {
            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.CallId}] {toolCall.Name}");
            return tool.InvokeFunction(toolCall);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="IToolCall"/>.
        /// </summary>
        /// <typeparam name="T">The expected signature return type.</typeparam>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="IToolCall"/> if you know the method is synchronous.</remarks>
        public static T InvokeToolCall<T>(this AssistantResponse assistant, IToolCall toolCall)
        {
            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.CallId}] {toolCall.Name}");
            return tool.InvokeFunction<T>(toolCall);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="IToolCall"/>.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        public static Task<string> InvokeToolCallAsync(this AssistantResponse assistant, IToolCall toolCall, CancellationToken cancellationToken = default)
        {
            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Name) ??
                throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.CallId}] {toolCall.Name}");
            return tool.InvokeFunctionAsync(toolCall, cancellationToken);
        }

        /// <summary>
        /// Invoke the assistant's tool function using the <see cref="IToolCall"/>.
        /// </summary>
        /// <typeparam name="T">The expected signature return type.</typeparam>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tool output result as <see cref="string"/>.</returns>
        public static Task<T> InvokeToolCallAsync<T>(this AssistantResponse assistant, IToolCall toolCall, CancellationToken cancellationToken = default)
        {
            var tool = assistant.Tools.FirstOrDefault(tool => tool.IsFunction && tool.Function.Name == toolCall.Name) ??
                       throw new InvalidOperationException($"Failed to find a valid tool for [{toolCall.CallId}] {toolCall.Name}");
            return tool.InvokeFunctionAsync<T>(toolCall, cancellationToken);
        }

        /// <summary>
        /// Calls the tool's function, with the provided arguments from the toolCall and returns the output.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <returns><see cref="ToolOutput"/>.</returns>
        /// <remarks>Only call this directly on your <see cref="IToolCall"/> if you know the method is synchronous.</remarks>
        public static ToolOutput GetToolOutput(this AssistantResponse assistant, IToolCall toolCall)
            => new(toolCall.CallId, assistant.InvokeToolCall(toolCall));

        /// <summary>
        /// Calls the tool's function, with the provided arguments from the toolCall and returns the output.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCall"><see cref="IToolCall"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ToolOutput"/>.</returns>
        public static async Task<ToolOutput> GetToolOutputAsync(this AssistantResponse assistant, IToolCall toolCall, CancellationToken cancellationToken = default)
        {
            var output = await assistant.InvokeToolCallAsync(toolCall, cancellationToken).ConfigureAwait(false);
            return new ToolOutput(toolCall.CallId, output);
        }

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="toolCalls">A collection of <see cref="IToolCall"/>s.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static async Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, IEnumerable<IToolCall> toolCalls, CancellationToken cancellationToken = default)
            => await Task.WhenAll(toolCalls.Select(toolCall => assistant.GetToolOutputAsync(toolCall, cancellationToken))).ConfigureAwait(false);

        /// <summary>
        /// Calls each tool's function, with the provided arguments from the toolCalls and returns the outputs.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="run">The <see cref="RunResponse"/> to complete the tool calls for.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ToolOutput"/>s.</returns>
        public static Task<IReadOnlyList<ToolOutput>> GetToolOutputsAsync(this AssistantResponse assistant, RunResponse run, CancellationToken cancellationToken = default)
            => GetToolOutputsAsync(assistant, run.RequiredAction.SubmitToolOutputs.ToolCalls, cancellationToken);

        #endregion Tools
    }
}
