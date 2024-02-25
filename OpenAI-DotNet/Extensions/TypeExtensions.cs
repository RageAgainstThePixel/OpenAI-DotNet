// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;

namespace OpenAI.Extensions
{
    internal static class TypeExtensions
    {
        public static JsonObject GenerateJsonSchema(this MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                return null;
            }

            var schema = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject()
            };
            var requiredParameters = new JsonArray();

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(parameter.Name))
                {
                    throw new InvalidOperationException($"Failed to find a valid parameter name for {methodInfo.DeclaringType}.{methodInfo.Name}()");
                }

                if (!parameter.HasDefaultValue)
                {
                    requiredParameters.Add(parameter.Name);
                }

                schema["properties"]![parameter.Name] = GenerateJsonSchema(parameter.ParameterType);
            }

            if (requiredParameters.Count > 0)
            {
                schema["required"] = requiredParameters;
            }

            return schema;
        }

        public static JsonObject GenerateJsonSchema(this Type type)
        {
            var schema = new JsonObject();

            if (type.IsEnum)
            {
                schema["type"] = "string";
                schema["enum"] = new JsonArray();

                foreach (var value in Enum.GetValues(type))
                {
                    schema["enum"].AsArray().Add(JsonNode.Parse(JsonSerializer.Serialize(value, OpenAIClient.JsonSerializationOptions)));
                }
            }
            else if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                schema["type"] = "array";
                schema["items"] = GenerateJsonSchema(type.GetElementType() ?? type.GetGenericArguments()[0]);
            }
            else if (type.IsClass && type != typeof(string))
            {
                schema["type"] = "object";
                var properties = type.GetProperties();
                var propertiesInfo = new JsonObject();
                var requiredProperties = new JsonArray();

                foreach (var property in properties)
                {
                    var propertyInfo = GenerateJsonSchema(property.PropertyType);
                    var functionPropertyAttribute = property.GetCustomAttribute<FunctionPropertyAttribute>();
                    var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    var propertyName = jsonPropertyAttribute?.Name ?? property.Name;

                    // override properties with values from function property attribute
                    if (functionPropertyAttribute != null)
                    {
                        propertyInfo["description"] = functionPropertyAttribute.Description;

                        if (functionPropertyAttribute.Required)
                        {
                            requiredProperties.Add(propertyName);
                        }

                        JsonNode defaultValue = null;

                        if (functionPropertyAttribute.DefaultValue != null)
                        {
                            defaultValue = JsonNode.Parse(JsonSerializer.Serialize(functionPropertyAttribute.DefaultValue, OpenAIClient.JsonSerializationOptions));
                            propertyInfo["default"] = defaultValue;
                        }

                        if (functionPropertyAttribute.PossibleValues is { Length: > 0 })
                        {
                            var enums = new JsonArray();

                            foreach (var value in functionPropertyAttribute.PossibleValues)
                            {
                                var @enum = JsonNode.Parse(JsonSerializer.Serialize(value, OpenAIClient.JsonSerializationOptions));

                                if (defaultValue == null)
                                {
                                    enums.Add(@enum);
                                }
                                else
                                {
                                    if (@enum != defaultValue)
                                    {
                                        enums.Add(@enum);
                                    }
                                }
                            }

                            if (defaultValue != null && !enums.Contains(defaultValue))
                            {
                                enums.Add(JsonNode.Parse(defaultValue.ToJsonString()));
                            }

                            propertyInfo["enum"] = enums;
                        }
                    }
                    else if (Nullable.GetUnderlyingType(property.PropertyType) == null)
                    {
                        requiredProperties.Add(propertyName);
                    }

                    propertiesInfo[propertyName] = propertyInfo;
                }

                schema["properties"] = propertiesInfo;

                if (requiredProperties.Count > 0)
                {
                    schema["required"] = requiredProperties;
                }
            }
            else
            {
                if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                {
                    schema["type"] = "integer";
                }
                else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                {
                    schema["type"] = "number";
                }
                else if (type == typeof(bool))
                {
                    schema["type"] = "boolean";
                }
                else if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
                {
                    schema["type"] = "string";
                    schema["format"] = "date-time";
                }
                else if (type == typeof(Guid))
                {
                    schema["type"] = "string";
                    schema["format"] = "uuid";
                }
                else
                {
                    schema["type"] = type.Name.ToLower();
                }
            }

            return schema;
        }
    }
}