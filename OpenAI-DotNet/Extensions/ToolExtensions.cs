// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class ToolExtensions
    {
        public static void ProcessTools<T>(this IEnumerable<Tool> tools, string toolChoice, out IReadOnlyList<T> toolList, out object activeTool) where T : ITool
        {
            var knownTools = tools?.ToList();
            toolList = knownTools?.ConvertTools<T>();

            if (toolList is { Count: > 0 })
            {
                if (string.IsNullOrWhiteSpace(toolChoice))
                {
                    activeTool = "auto";
                }
                else
                {
                    if (!toolChoice.Equals("none") &&
                        !toolChoice.Equals("required") &&
                        !toolChoice.Equals("auto"))
                    {
                        var tool = knownTools?.Where(t => t.IsFunction).FirstOrDefault(t => t.Function.Name.Contains(toolChoice)) ??
                                   throw new ArgumentException($"The specified tool choice '{toolChoice}' was not found in the list of tools");
                        activeTool = new { type = "function", function = new { name = tool.Function.Name } };
                    }
                    else
                    {
                        activeTool = toolChoice;
                    }
                }

                foreach (var tool in toolList)
                {
                    if (tool is Function { Arguments: not null } function)
                    {
                        // just in case clear any lingering func args.
                        function.Arguments = null;
                    }
                }
            }
            else
            {
                activeTool = string.IsNullOrWhiteSpace(toolChoice) ? "none" : toolChoice;
            }
        }

        public static IReadOnlyList<T> ConvertTools<T>(this IReadOnlyList<Tool> tools) where T : ITool
        {
            var result = new List<T>();

            if (typeof(T) == typeof(Tool))
            {
                // cannot add ITools to List<Tool>
                result.AddRange((IEnumerable<T>)tools.Where(tool =>
                {
                    if (tool.IsFunction)
                    {
                        tool.Function.Type = null;
                    }

                    return !tool.IsReference;
                }));
            }
            else
            {
                // add all ITools
                result.AddRange((IEnumerable<T>)tools.Where(tool => tool.IsReference).Select(tool => tool.Reference));
                // finally add all custom functions
                result.AddRange((IEnumerable<T>)tools.Where(tool => tool.IsFunction).Select(tool =>
                {
                    tool.Function.Type = "function";
                    return tool.Function;
                }));
            }

            return result;
        }


        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <param name="toolCall">Tool call to invoke for output.</param>
        /// <returns>The result of the function as json.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public static string InvokeFunction(this IToolCall toolCall)
            => TryGetToolCache(toolCall, out var tool)
                ? tool.InvokeFunction(toolCall)
                : throw new InvalidOperationException($"Tool \"{toolCall.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="toolCall">Tool call to invoke for output.</param>
        /// <returns>The result of the function.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public static T InvokeFunction<T>(this IToolCall toolCall)
            => TryGetToolCache(toolCall, out var tool)
                ? tool.InvokeFunction<T>(toolCall)
                : throw new InvalidOperationException($"Tool \"{toolCall.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <param name="toolCall">Tool call to invoke for output.</param>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function as json.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public static async Task<string> InvokeFunctionAsync(this IToolCall toolCall, CancellationToken cancellationToken = default)
            => TryGetToolCache(toolCall, out var tool)
                ? await tool.InvokeFunctionAsync(toolCall, cancellationToken)
                : throw new InvalidOperationException($"Tool \"{toolCall.Name}\" is not registered!");

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="toolCall">Tool call to invoke for output.</param>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function.</returns>
        /// <exception cref="InvalidOperationException">If tool is not a function or tool is not registered.</exception>
        public static async Task<T> InvokeFunctionAsync<T>(this IToolCall toolCall, CancellationToken cancellationToken = default)
        {
            return TryGetToolCache(toolCall, out var tool)
                ? await tool.InvokeFunctionAsync<T>(toolCall, cancellationToken)
                : throw new InvalidOperationException($"Tool \"{toolCall.Name}\" is not registered!");
        }

        private static bool TryGetToolCache(IToolCall toolCall, out Tool tool)
        {
            tool = null;

            if (toolCache.TryGetValue(toolCall.Name, out tool))
            {
                return true;
            }

            if (Tool.TryGetTool(toolCall, out tool))
            {
                toolCache[toolCall.Name] = tool;
                return true;
            }

            return false;
        }

        private static readonly Dictionary<string, Tool> toolCache = new();
    }
}
