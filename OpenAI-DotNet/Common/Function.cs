// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    /// <summary>
    /// <see href="https://platform.openai.com/docs/guides/function-calling"/>
    /// </summary>
    public sealed class Function
    {
        public Function() { }

        private const string NameRegex = "^[a-zA-Z0-9_-]{1,64}$";

        /// <summary>
        /// Creates a new function description to insert into a chat conversation.
        /// </summary>
        /// <param name="name">
        /// Required. The name of the function to generate arguments for based on the context in a message.<br/>
        /// May contain a-z, A-Z, 0-9, underscores and dashes, with a maximum length of 64 characters. Recommended to not begin with a number or a dash.
        /// </param>
        /// <param name="description">
        /// An optional description of the function, used by the API to determine if it is useful to include in the response.
        /// </param>
        /// <param name="parameters">
        /// An optional JSON object describing the parameters of the function that the model can generate.
        /// </param>
        /// <param name="arguments">
        /// An optional JSON object describing the arguments to use when invoking the function.
        /// </param>
        public Function(string name, string description = null, JsonNode parameters = null, JsonNode arguments = null)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, NameRegex))
            {
                throw new ArgumentException($"The name of the function does not conform to naming standards: {NameRegex}");
            }

            Name = name;
            Description = description;
            Parameters = parameters;
            Arguments = arguments;
        }

        internal Function(Function other) => CopyFrom(other);

        internal Function(string name, string description, JsonObject parameters, MethodInfo method)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, NameRegex))
            {
                throw new ArgumentException($"The name of the function does not conform to naming standards: {NameRegex}");
            }

            Name = name;
            Description = description;
            Parameters = parameters;
            functionCache[Name] = method;
        }

        /// <summary>
        /// The name of the function to generate arguments for.<br/>
        /// May contain a-z, A-Z, 0-9, and underscores and dashes, with a maximum length of 64 characters.
        /// Recommended to not begin with a number or a dash.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The optional description of the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description { get; private set; }

        private string parametersString;

        private JsonNode parameters;

        /// <summary>
        /// The optional parameters of the function.
        /// Describe the parameters that the model should generate in JSON schema format (json-schema.org).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parameters")]
        public JsonNode Parameters
        {
            get
            {
                if (parameters == null &&
                    !string.IsNullOrWhiteSpace(parametersString))
                {
                    parameters = JsonNode.Parse(parametersString);
                }

                return parameters;
            }
            private set => parameters = value;
        }

        private string argumentsString;

        private JsonNode arguments;

        /// <summary>
        /// The arguments to use when calling the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        public JsonNode Arguments
        {
            get
            {
                if (arguments == null &&
                    !string.IsNullOrWhiteSpace(argumentsString))
                {
                    arguments = JsonValue.Create(argumentsString);
                }

                return arguments;
            }
            internal set => arguments = value;
        }

        internal void CopyFrom(Function other)
        {
            if (!string.IsNullOrWhiteSpace(other.Name))
            {
                Name = other.Name;
            }

            if (!string.IsNullOrWhiteSpace(other.Description))
            {
                Description = other.Description;
            }

            if (other.Arguments != null)
            {
                argumentsString += other.Arguments.ToString();
            }

            if (other.Parameters != null)
            {
                parametersString += other.Parameters.ToString();
            }
        }

        #region Function Inoking Utilities

        private static readonly Dictionary<string, MethodInfo> functionCache = new();

        public string Invoke()
        {
            var (method, invokeArgs) = ValidateFunctionArguments();
            var result = method.Invoke(null, invokeArgs);
            return result == null ? string.Empty : JsonSerializer.Serialize(new { result }, OpenAIClient.JsonSerializationOptions);
        }

        public async Task<string> InvokeAsync(CancellationToken cancellationToken = default)
        {
            var (method, invokeArgs) = ValidateFunctionArguments(cancellationToken);
            var task = (Task)method.Invoke(null, invokeArgs);

            if (task is null)
            {
                throw new InvalidOperationException($"The function {Name} did not return a Task.");
            }

            await task.ConfigureAwait(false);

            if (method.ReturnType == typeof(Task))
            {
                return string.Empty;
            }

            var result = method.ReturnType.GetProperty(nameof(Task<object>.Result))?.GetValue(task);
            return result == null ? string.Empty : JsonSerializer.Serialize(new { result }, OpenAIClient.JsonSerializationOptions);
        }

        private (MethodInfo method, object[] invokeArgs) ValidateFunctionArguments(CancellationToken cancellationToken = default)
        {
            if (Parameters != null && Arguments == null)
            {
                throw new ArgumentException($"Function {Name} has parameters but no arguments are set.");
            }

            if (!functionCache.TryGetValue(Name, out var method))
            {
                if (!Name.Contains('_'))
                {
                    throw new InvalidOperationException($"Failed to lookup and invoke function \"{Name}\"");
                }

                var type = Type.GetType(Name[..Name.LastIndexOf('_')].Replace('_', '.'));

                if (type == null)
                {
                    throw new InvalidOperationException($"Failed to find a valid type for {Name}");
                }

                method = type.GetMethod(Name[(Name.LastIndexOf('_') + 1)..].Replace('_', '.'));

                if (method == null)
                {
                    throw new InvalidOperationException($"Failed to find a valid method for {Name}");
                }

                functionCache[Name] = method;
            }

            var requestedArgs = JsonSerializer.Deserialize<Dictionary<string, object>>(Arguments.ToString());
            var methodParams = method.GetParameters();
            var invokeArgs = new object[methodParams.Length];

            for (var i = 0; i < methodParams.Length; i++)
            {
                var parameter = methodParams[i];

                if (parameter.Name == null)
                {
                    throw new InvalidOperationException($"Failed to find a valid parameter name for {method.DeclaringType}.{method.Name}()");
                }

                if (requestedArgs.TryGetValue(parameter.Name, out var value))
                {
                    if (parameter.ParameterType == typeof(CancellationToken))
                    {
                        invokeArgs[i] = cancellationToken;
                    }
                    else if (value is JsonElement element)
                    {
                        invokeArgs[i] = JsonSerializer.Deserialize(element.GetRawText(), parameter.ParameterType, OpenAIClient.JsonSerializationOptions);
                    }
                    else
                    {
                        invokeArgs[i] = value;
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    invokeArgs[i] = parameter.DefaultValue;
                }
                else
                {
                    throw new ArgumentException($"Missing argument for parameter '{parameter.Name}'");
                }
            }

            return (method, invokeArgs);
        }

        #endregion Function Inoking Utilities
    }
}
