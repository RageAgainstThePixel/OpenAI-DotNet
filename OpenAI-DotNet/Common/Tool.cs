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

        private static List<Tool> toolCache;

        public static IReadOnlyList<Tool> GetAllAvailableTools(bool includeDefaults = true)
        {
            if (toolCache != null) { return toolCache; }

            var tools = new List<Tool>();

            if (includeDefaults)
            {
                tools.Add(Retrieval);
                tools.Add(CodeInterpreter);
            }

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
            toolCache = tools;
            return tools;
        }

        public static Tool GetOrCreateTool(Type type, string methodName)
        {
            var knownTools = GetAllAvailableTools(false);
            var method = type.GetMethod(methodName);

            if (method == null)
            {
                throw new InvalidOperationException($"Failed to find a valid method for {type.FullName}.{methodName}()");
            }

            var functionAttribute = method.GetCustomAttribute<FunctionAttribute>();

            if (functionAttribute == null)
            {
                throw new InvalidOperationException($"{type.FullName}.{methodName}() must have a {nameof(FunctionAttribute)} decorator!");
            }

            var functionName = $"{type.FullName}.{method.Name}".Replace('.', '_');

            foreach (var knownTool in knownTools)
            {
                if (knownTool.Type == "function" && knownTool.Function.Name == functionName)
                {
                    return knownTool;
                }
            }

            var tool = new Tool(new Function(functionName, functionAttribute.Description, method.GenerateJsonSchema(), method));
            toolCache.Add(tool);
            return tool;
        }
    }
}