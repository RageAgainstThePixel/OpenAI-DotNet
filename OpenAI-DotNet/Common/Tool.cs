// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <returns>The result of the function as json.</returns>
        public string InvokeFunction() => Function.Invoke();

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <returns>The result of the function.</returns>
        public T InvokeFunction<T>() => Function.Invoke<T>();

        /// <summary>
        /// Invokes the function and returns the result as json.
        /// </summary>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function as json.</returns>
        public async Task<string> InvokeFunctionAsync(CancellationToken cancellationToken = default)
            => await Function.InvokeAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Invokes the function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to.</typeparam>
        /// <param name="cancellationToken">Optional, A token to cancel the request.</param>
        /// <returns>The result of the function.</returns>
        public async Task<T> InvokeFunctionAsync<T>(CancellationToken cancellationToken = default)
            => await Function.InvokeAsync<T>(cancellationToken).ConfigureAwait(false);

        private static readonly List<Tool> toolCache = new()
        {
            Retrieval,
            CodeInterpreter
        };

        /// <summary>
        /// Clears the tool cache of all previously registered tools.
        /// </summary>
        public static void ClearRegisteredTools()
        {
            toolCache.Clear();
            Function.ClearFunctionCache();
            toolCache.Add(CodeInterpreter);
            toolCache.Add(Retrieval);
        }

        /// <summary>
        /// Checks if tool exists in cache.
        /// </summary>
        /// <param name="tool">The tool to check.</param>
        /// <returns>True, if the tool is already registered in the tool cache.</returns>
        public static bool IsToolRegistered(Tool tool)
            => toolCache.Any(knownTool =>
                knownTool.Type == "function" &&
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

            if (tool.Type != "function")
            {
                throw new InvalidOperationException("Only function tools can be registered.");
            }

            toolCache.Add(tool);
            return true;

        }

        private static bool TryGetTool(string name, object instance, out Tool tool)
        {
            foreach (var knownTool in toolCache.Where(knownTool =>
                         knownTool.Type == "function" &&
                         knownTool.Function.Name == name &&
                         ReferenceEquals(knownTool, instance)))
            {
                tool = knownTool;
                return true;
            }

            tool = null;
            return false;
        }

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

            if (forceUpdate || toolCache.All(tool => tool.Type != "function"))
            {
                var tools = new List<Tool>();
                tools.AddRange(
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    from method in type.GetMethods()
                    where method.IsStatic
                    let functionAttribute = method.GetCustomAttribute<FunctionAttribute>()
                    where functionAttribute != null
                    let name = $"{type.FullName}.{method.Name}".Replace('.', '_')
                    let description = functionAttribute.Description
                    select new Function(name, description, method)
                    into function
                    select new Tool(function));

                foreach (var newTool in tools.Where(tool =>
                             !toolCache.Any(knownTool =>
                                 knownTool.Type == "function" && knownTool.Function.Name == tool.Function.Name && knownTool.Function.Instance == null)))
                {
                    toolCache.Add(newTool);
                }
            }

            return !includeDefaults
                ? toolCache.Where(tool => tool.Type == "function").ToList()
                : toolCache;
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

            var functionName = $"{type.FullName}.{method.Name}".Replace('.', '_');

            if (TryGetTool(functionName, null, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.GetOrCreateFunction(functionName, description ?? string.Empty, method));
            toolCache.Add(tool);
            return tool;
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

            var functionName = $"{type.FullName}.{method.Name}".Replace('.', '_');

            if (TryGetTool(functionName, instance, out var tool))
            {
                return tool;
            }

            tool = new Tool(new Function(functionName, description ?? string.Empty, method, instance));
            toolCache.Add(tool);
            return tool;
        }

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

        public static Tool FromFunc<T1, T2, TResult>(string name, Func<T1, T2, TResult> function,
            string description = null)
        {
            if (TryGetTool(name, function, out var tool))
            {
                return tool;
            }

            tool = new Tool(Function.FromFunc(name, function, description));
            toolCache.Add(tool);
            return tool;
        }

        public static Tool FromFunc<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> function,
            string description = null)
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