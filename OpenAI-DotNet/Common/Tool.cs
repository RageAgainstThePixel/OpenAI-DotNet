// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public sealed class Tool
    {
        public Tool() { }

        public Tool(Tool other) => CopyFrom(other);

        public Tool(Function function)
        {
            Function = function;
            Type = nameof(function);
        }

        public static implicit operator Tool(Function function) => new(function);

        public static Tool Retrieval { get; } = new() { Type = "retrieval" };

        public static Tool CodeInterpreter { get; } = new() { Type = "code_interpreter" };

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("function")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Function Function { get; private set; }

        public string InvokeFunction() => Function.Invoke();

        public async Task<string> InvokeFunctionAsync(CancellationToken cancellationToken = default)
            => await Function.InvokeAsync(cancellationToken).ConfigureAwait(false);

        internal void CopyFrom(Tool other)
        {
            if (!string.IsNullOrWhiteSpace(other?.Id))
            {
                Id = other.Id;
            }

            if (other is { Index: not null })
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other?.Type))
            {
                Type = other.Type;
            }

            if (other?.Function != null)
            {
                if (Function == null)
                {
                    Function = new Function(other.Function);
                }
                else
                {
                    Function.CopyFrom(other.Function);
                }
            }
        }

        private static List<Tool> toolCache = new()
        {
            Retrieval,
            CodeInterpreter
        };

        /// <summary>
        /// Gets a list of all available tools.
        /// </summary>
        /// <remarks>
        /// This method will scan all assemblies for methods decorated with the <see cref="FunctionAttribute"/>.
        /// </remarks>
        /// <param name="includeDefaults">Optional, Whether to include the default tools (Retrieval and CodeInterpreter).</param>
        /// <param name="forceUpdate">Optional, Whether to force an update of the tool cache.</param>
        /// <returns>A list of all available tools.</returns>
        public static IReadOnlyList<Tool> GetAllAvailableTools(bool includeDefaults = true, bool forceUpdate = false)
        {
            if (forceUpdate || toolCache.All(tool => tool.Type != "function"))
            {
                var tools = new List<Tool>();
                tools.AddRange(
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    from method in type.GetMethods()
                    let functionAttribute = method.GetCustomAttribute<FunctionAttribute>()
                    where functionAttribute != null
                    let name = $"{type.FullName}.{method.Name}".Replace('.', '_')
                    let description = functionAttribute.Description
                    let parameters = method.GenerateJsonSchema()
                    select new Function(name, description, parameters, method)
                    into function
                    select new Tool(function));

                foreach (var newTool in tools.Where(knownTool => toolCache.All(tool => tool.Type == "function" && tool.Function.Name != knownTool.Function.Name)))
                {
                    toolCache.Add(newTool);
                }
            }

            return !includeDefaults
                ? toolCache.Where(tool => tool.Type == "function").ToList()
                : toolCache;
        }

        /// <summary>
        /// Get or create a tool from a method.
        /// </summary>
        /// <remarks>
        /// If the tool already exists, it will be returned. Otherwise, a new tool will be created.<br/>
        /// The method doesn't need to be decorated with the <see cref="FunctionAttribute"/>.<br/>
        /// </remarks>
        /// <param name="type">The type containing the method.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="description">Optional, The description of the method.</param>
        /// <returns>The tool for the method.</returns>
        public static Tool GetOrCreateTool(Type type, string methodName, string description = null)
        {
            var method = type.GetMethod(methodName) ??
                throw new InvalidOperationException($"Failed to find a valid method for {type.FullName}.{methodName}()");
            var functionName = $"{type.FullName}.{method.Name}".Replace('.', '_');

            foreach (var knownTool in toolCache.Where(knownTool => knownTool.Type == "function" && knownTool.Function.Name == functionName))
            {
                return knownTool;
            }

            var tool = new Tool(new Function(functionName, description ?? string.Empty, method.GenerateJsonSchema(), method));
            toolCache.Add(tool);
            return tool;
        }
    }
}