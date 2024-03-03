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

                schema["properties"]![parameter.Name] = GenerateJsonSchema(parameter.ParameterType, schema);

                var functionParameterAttribute = parameter.GetCustomAttribute<FunctionParameterAttribute>();

                if (functionParameterAttribute != null)
                {
                    schema["properties"]![parameter.Name]!["description"] = functionParameterAttribute.Description;
                }
            }

            if (requiredParameters.Count > 0)
            {
                schema["required"] = requiredParameters;
            }

            return schema;
        }

        public static JsonObject GenerateJsonSchema(this Type type, JsonObject rootSchema)
        {
            var schema = new JsonObject();

            if (!type.IsPrimitive &&
                type != typeof(Guid) &&
                type != typeof(DateTime) &&
                type != typeof(DateTimeOffset) &&
                rootSchema["definitions"] != null &&
                rootSchema["definitions"].AsObject().ContainsKey(type.FullName))
            {
                return new JsonObject { ["$ref"] = $"#/definitions/{type.FullName}" };
            }

            if (type == typeof(string) || type == typeof(char))
            {
                schema["type"] = "string";
            }
            else if (type == typeof(int) ||
                     type == typeof(long) ||
                     type == typeof(uint) ||
                     type == typeof(byte) ||
                     type == typeof(sbyte) ||
                     type == typeof(ulong) ||
                     type == typeof(short) ||
                     type == typeof(ushort))
            {
                schema["type"] = "integer";
            }
            else if (type == typeof(float) ||
                     type == typeof(double) ||
                     type == typeof(decimal))
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
            else if (type.IsEnum)
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
                var elementType = type.GetElementType() ?? type.GetGenericArguments()[0];

                if (rootSchema["definitions"] != null &&
                    rootSchema["definitions"].AsObject().ContainsKey(elementType.FullName))
                {
                    schema["items"] = new JsonObject { ["$ref"] = $"#/definitions/{elementType.FullName}" };
                }
                else
                {
                    schema["items"] = GenerateJsonSchema(elementType, rootSchema);
                }
            }
            else
            {
                schema["type"] = "object";
                rootSchema["definitions"] ??= new JsonObject();
                rootSchema["definitions"][type.FullName] = new JsonObject();

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var members = new List<MemberInfo>(properties.Length + fields.Length);
                members.AddRange(properties);
                members.AddRange(fields);

                var memberInfo = new JsonObject();
                var memberProperties = new JsonArray();

                foreach (var member in members)
                {
                    var memberType = GetMemberType(member);
                    var functionPropertyAttribute = member.GetCustomAttribute<FunctionPropertyAttribute>();
                    var jsonPropertyAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();
                    var jsonIgnoreAttribute = member.GetCustomAttribute<JsonIgnoreAttribute>();
                    var propertyName = jsonPropertyAttribute?.Name ?? member.Name;

                    JsonObject propertyInfo;

                    if (rootSchema["definitions"] != null &&
                        rootSchema["definitions"].AsObject().ContainsKey(memberType.FullName))
                    {
                        propertyInfo = new JsonObject { ["$ref"] = $"#/definitions/{memberType.FullName}" };
                    }
                    else
                    {
                        propertyInfo = GenerateJsonSchema(memberType, rootSchema);
                    }

                    // override properties with values from function property attribute
                    if (functionPropertyAttribute != null)
                    {
                        propertyInfo["description"] = functionPropertyAttribute.Description;

                        if (functionPropertyAttribute.Required)
                        {
                            memberProperties.Add(propertyName);
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
                                enums.Add(JsonNode.Parse(defaultValue.ToJsonString(OpenAIClient.JsonSerializationOptions)));
                            }

                            propertyInfo["enum"] = enums;
                        }
                    }
                    else if (jsonIgnoreAttribute != null)
                    {
                        // only add members that are required
                        switch (jsonIgnoreAttribute.Condition)
                        {
                            case JsonIgnoreCondition.Never:
                            case JsonIgnoreCondition.WhenWritingDefault:
                                memberProperties.Add(propertyName);
                                break;
                            case JsonIgnoreCondition.Always:
                            case JsonIgnoreCondition.WhenWritingNull:
                            default:
                                memberProperties.Remove(propertyName);
                                break;
                        }
                    }
                    else if (Nullable.GetUnderlyingType(memberType) == null)
                    {
                        memberProperties.Add(propertyName);
                    }

                    memberInfo[propertyName] = propertyInfo;
                }

                schema["properties"] = memberInfo;

                if (memberProperties.Count > 0)
                {
                    schema["required"] = memberProperties;
                }

                rootSchema["definitions"] ??= new JsonObject();
                rootSchema["definitions"][type.FullName] = schema;
                return new JsonObject { ["$ref"] = $"#/definitions/{type.FullName}" };
            }

            return schema;
        }

        private static Type GetMemberType(MemberInfo member)
            => member switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw new ArgumentException($"{nameof(MemberInfo)} must be of type {nameof(FieldInfo)}, {nameof(PropertyInfo)}", nameof(member))
            };
    }
}