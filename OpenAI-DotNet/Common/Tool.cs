// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public sealed class Tool : IAppendable<Tool>
    {
        public Tool() { }

        public Tool(Tool other) => AppendFrom(other);

        public Tool(Function function)
        {
            Function = function;
            Type = nameof(function);
        }

        public Tool(string toolCallId, string functionName, JsonNode functionArguments)
        {
            Function = new Function(functionName, arguments: functionArguments);
            Type = "function";
            Id = toolCallId;
        }

        public Tool(FileSearchOptions fileSearchOptions)
        {
            Type = "file_search";
            FileSearchOptions = fileSearchOptions;
        }

        public static implicit operator Tool(Function function) => new(function);

        public static implicit operator Tool(FileSearchOptions fileSearchOptions) => new(fileSearchOptions);

        [Obsolete("Use FileSearch")]
        public static Tool Retrieval { get; } = new() { Type = "file_search" };

        public static Tool FileSearch { get; } = new() { Type = "file_search" };

        public static Tool CodeInterpreter { get; } = new() { Type = "code_interpreter" };

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonIgnore]
        public bool IsFunction => Type == "function";

        [JsonInclude]
        [JsonPropertyName("function")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Function Function { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_search")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FileSearchOptions FileSearchOptions { get; private set; }

        public void AppendFrom(Tool other)
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

            if (other.FileSearchOptions != null)
            {
                FileSearchOptions = other.FileSearchOptions;
            }
        }

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <returns>The result of the function as json.</returns>
        /// <remarks>Only call this directly if you know the method is synchronous.</remarks>
        public string InvokeFunction()
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException("This tool is not a function.");
            }

            return Function.Invoke();
        }

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <returns>The result of the function.</returns>
        /// <remarks>Only call this directly if you know the method is synchronous.</remarks>
        public T InvokeFunction<T>()
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException("This tool is not a function.");
            }

            return Function.Invoke<T>();
        }

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function as json.</returns>
        public async Task<string> InvokeFunctionAsync(CancellationToken cancellationToken = default)
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException("This tool is not a function.");
            }

            return await Function.InvokeAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function.</returns>
        public async Task<T> InvokeFunctionAsync<T>(CancellationToken cancellationToken = default)
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException("This tool is not a function.");
            }

            return await Function.InvokeAsync<T>(cancellationToken).ConfigureAwait(false);
        }

        #region Tool Cache

        private static readonly List<Tool> toolCache = new()
        {
            FileSearch,
            CodeInterpreter
        };

        /// <summary>
        /// Gets a list of all available tools.
        /// </summary>
        /// <remarks>
        /// This method will scan all assemblies for static methods decorated with the <see cref="FunctionAttribute"/>.
        /// </remarks>
        /// <param name="includeDefaults">Optional, Whether to include the default tools (Retrieval and CodeInterpreter).</param>
        /// <param name="forceUpdate">Optional, Whether to force an update of the tool cache.</param>
        /// <param name="clearCache">Optional, whether to force the tool cache to be cleared before updating.</param>
        /// <returns>A list of all available tools.</returns>
        public static IReadOnlyList<Tool> GetAllAvailableTools(bool includeDefaults = true, bool forceUpdate = false, bool clearCache = false)
        {
            if (clearCache)
            {
                ClearRegisteredTools();
            }

            if (forceUpdate || toolCache.All(tool => !tool.IsFunction))
            {
                var tools = new List<Tool>();
                tools.AddRange(
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    from method in type.GetMethods()
                    where method.IsStatic
                    let functionAttribute = method.GetCustomAttribute<FunctionAttribute>()
                    where functionAttribute != null
                    let name = GetFunctionName(type, method)
                    let description = functionAttribute.Description
                    select Function.GetOrCreateFunction(name, description, method)
                    into function
                    select new Tool(function));

                foreach (var newTool in tools.Where(tool =>
                             !toolCache.Any(knownTool =>
                                 knownTool.IsFunction && knownTool.Function.Name == tool.Function.Name && knownTool.Function.Instance == null)))
                {
                    toolCache.Add(newTool);
                }
            }

            return !includeDefaults
                ? toolCache.Where(tool => tool.IsFunction).ToList()
                : toolCache;
        }

        /// <summary>
        /// Clears the tool cache of all previously registered tools.
        /// </summary>
        public static void ClearRegisteredTools()
        {
            toolCache.Clear();
            Function.ClearFunctionCache();
            toolCache.Add(CodeInterpreter);
            toolCache.Add(FileSearch);
        }

        /// <summary>
        /// Checks if tool exists in cache.
        /// </summary>
        /// <param name="tool">The tool to check.</param>
        /// <returns>True, if the tool is already registered in the tool cache.</returns>
        public static bool IsToolRegistered(Tool tool)
            => toolCache.Any(knownTool =>
                knownTool.IsFunction &&
                knownTool.Function.Name == tool.Function.Name &&
                ReferenceEquals(knownTool.Function.Instance, tool.Function.Instance));

        /// <summary>
        /// Tries to register a tool into the Tool cache.
        /// </summary>
        /// <param name="tool">The tool to register.</param>
        /// <returns>True, if the tool was added to the cache.</returns>
        public static bool TryRegisterTool(Tool tool)
        {
            if (IsToolRegistered(tool))
            {
                return false;
            }

            if (!tool.IsFunction)
            {
                throw new InvalidOperationException("Only function tools can be registered.");
            }

            toolCache.Add(tool);
            return true;
        }

        /// <summary>
        /// Tries to remove a tool from the Tool cache.
        /// </summary>
        /// <param name="tool">The tool to remove.</param>
        /// <returns>True, if the tool was removed from the cache.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool TryUnregisterTool(Tool tool)
        {
            if (!IsToolRegistered(tool))
            {
                return false;
            }

            if (!tool.IsFunction)
            {
                throw new InvalidOperationException("Only function tools can be unregistered.");
            }

            return Function.TryRemoveFunction(tool.Function.Name) && toolCache.Remove(tool);
        }

        /// <summary>
        /// Get or create a tool from a static method.
        /// </summary>
        /// <remarks>
        /// If the tool already exists, it will be returned. Otherwise, a new tool will be created.<br/>
        /// The method doesn't need to be decorated with the <see cref="FunctionAttribute"/>.<br/>
        /// </remarks>
        /// <param name="type">The type containing the static method.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="description">Optional, The description of the method.</param>
        /// <returns>The tool for the method.</returns>
        public static Tool GetOrCreateTool(Type type, string methodName, string description = null)
        {
            var method = type.GetMethod(methodName) ??
                throw new InvalidOperationException($"Failed to find a valid method for {type.FullName}.{methodName}()");

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method {type.FullName}.{methodName}() must be static. Use GetOrCreateTool(object instance, string methodName) instead.");
            }

            return GetOrCreateToolInternal(type, method, description);
        }

        /// <summary>
        /// Get or create a tool from a method of an instance of an object.
        /// </summary>
        /// <remarks>
        /// If the tool already exists, it will be returned. Otherwise, a new tool will be created.<br/>
        /// The method doesn't need to be decorated with the <see cref="FunctionAttribute"/>.<br/>
        /// </remarks>
        /// <param name="instance">The instance of the object containing the method.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="description">Optional, The description of the method.</param>
        /// <returns>The tool for the method.</returns>
        public static Tool GetOrCreateTool(object instance, string methodName, string description = null)
        {
            var type = instance.GetType();
            var method = type.GetMethod(methodName) ??
                throw new InvalidOperationException($"Failed to find a valid method for {type.FullName}.{methodName}()");
            return GetOrCreateToolInternal(type, method, description, instance);
        }

        private static Tool GetOrCreateToolInternal(Type type, MethodInfo method, string description, object instance = null)
        {
            var functionName = GetFunctionName(type, method);

            if (TryGetTool(functionName, instance, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.GetOrCreateFunction(functionName, description ?? string.Empty, method, instance));
            toolCache.Add(tool);
            return tool;
        }

        private static bool TryGetTool(string name, object instance, out Tool tool)
        {
            foreach (var knownTool in toolCache.Where(knownTool =>
                         knownTool.IsFunction && knownTool.Function.Name == name &&
                         ReferenceEquals(knownTool.Function.Instance, instance)))
            {
                tool = knownTool;
                return true;
            }

            tool = null;
            return false;
        }

        private static string GetFunctionName(Type type, MethodInfo methodInfo)
        {
            // todo possibly use string hash instead to mitigate long names?
            // todo possibly use AssemblyQualifiedName?
            return $"{type.FullName}.{methodInfo.Name}".Replace('.', '_');
        }

        #endregion Tool Cache

        #region Func<,> Overloads

        public static Tool FromFunc<TResult>(string name, Func<TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, TResult>(string name, Func<T1, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, TResult>(string name, Func<T1, T2, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, TResult>(string name, Func<T1, T2, T3, T4, T5, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }


        public static Tool FromFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(string name, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> function, string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        #endregion Func<,> Overloads
    }
}
