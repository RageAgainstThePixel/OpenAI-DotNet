// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public sealed class ToolCall : IAppendable<ToolCall>
    {
        public ToolCall() { }

        public ToolCall(string toolCallId, string functionName, JsonNode functionArguments = null)
        {
            Id = toolCallId;
            Function = new Function(functionName, arguments: functionArguments);
            Type = "function";
        }

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("function")]
        public Function Function { get; private set; }

        [JsonIgnore]
        public bool IsFunction => Type == "function";

        public void AppendFrom(ToolCall other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(other.Id))
            {
                Id = other.Id;
            }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other.Type))
            {
                Type = other.Type;
            }

            if (other.Function != null)
            {
                if (Function == null)
                {
                    Function = new Function(other.Function);
                }
                else
                {
                    Function.AppendFrom(other.Function);
                }
            }
        }

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <returns>The result of the function as json.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public string InvokeFunction()
            => TryGetToolCache(this, out var tool)
                ? tool.InvokeFunction(this)
                : throw new InvalidOperationException($"Tool \"{Function.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <returns>The result of the function.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public T InvokeFunction<T>()
            => TryGetToolCache(this, out var tool)
                ? tool.InvokeFunction<T>(this)
                : throw new InvalidOperationException($"Tool \"{Function.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function as json.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public async Task<string> InvokeFunctionAsync(CancellationToken cancellationToken = default)
            => TryGetToolCache(this, out var tool)
                ? await tool.InvokeFunctionAsync(this, cancellationToken)
                : throw new InvalidOperationException($"Tool \"{Function.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public async Task<T> InvokeFunctionAsync<T>(CancellationToken cancellationToken = default)
        {
            return TryGetToolCache(this, out var tool)
                ? await tool.InvokeFunctionAsync<T>(this, cancellationToken)
                : throw new InvalidOperationException($"Tool \"{Function.Name}\" is not registered!");
        }

        private static bool TryGetToolCache(ToolCall toolCall, out Tool tool)
        {
            tool = null;

            if (toolCache.TryGetValue(toolCall.Function.Name, out tool))
            {
                return true;
            }

            if (Tool.TryGetTool(toolCall, out tool))
            {
                toolCache[toolCall.Function.Name] = tool;
                return true;
            }

            return false;
        }

        private static readonly Dictionary<string, Tool> toolCache = new();
    }
}
