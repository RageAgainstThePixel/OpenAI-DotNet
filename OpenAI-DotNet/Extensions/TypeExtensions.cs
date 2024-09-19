// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;

namespace OpenAI.Extensions
{
    internal static class TypeExtensions
    {
        public static JsonObject GenerateJsonSchema(this MethodInfo methodInfo, JsonSerializerOptions options = null)
        {
            var schema = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject()
            };
            var requiredParameters = new JsonArray();
            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(CancellationToken)) { continue; }

                if (string.IsNullOrWhiteSpace(parameter.Name))
                {
                    throw new InvalidOperationException($"Failed to find a valid parameter name for {methodInfo.DeclaringType}.{methodInfo.Name}()");
                }

                if (!parameter.HasDefaultValue)
                {
                    requiredParameters.Add(parameter.Name);
                }

                schema["properties"]![parameter.Name] = GenerateJsonSchema(parameter.ParameterType, schema, options);

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

            schema["additionalProperties"] = false;
            return schema;
        }

        public static JsonObject GenerateJsonSchema(this Type type, JsonSerializerOptions options = null)
        {
            var schema = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject()
            };

            var requiredProperties = new JsonArray();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var property in properties)
            {
                var propertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                var propertyName = propertyNameAttribute?.Name ?? property.Name;
                requiredProperties.Add(propertyName);
                schema["properties"]![propertyName] = GenerateJsonSchema(property.PropertyType, schema, options);
            }

            if (requiredProperties.Count > 0)
            {
                schema["required"] = requiredProperties;
            }

            schema["additionalProperties"] = false;
            return schema;
        }

        public static JsonObject GenerateJsonSchema(this Type type, JsonObject rootSchema, JsonSerializerOptions options = null)
        {
            options ??= OpenAIClient.JsonSerializationOptions;
            var schema = new JsonObject();
            type = UnwrapNullableType(type);

            if (!type.IsPrimitive &&
                type != typeof(Guid) &&
                type != typeof(DateTime) &&
                type != typeof(DateTimeOffset) &&
                rootSchema["definitions"] != null &&
                rootSchema["definitions"].AsObject().ContainsKey(type.FullName!))
            {
                return new JsonObject { ["$ref"] = $"#/definitions/{type.FullName}" };
            }

            if (type.TryGetSimpleTypeSchema(out var schemaType))
            {
                schema["type"] = schemaType;

                if (type == typeof(DateTime) ||
                    type == typeof(DateTimeOffset))
                {
                    schema["format"] = "date-time";
                }
                else if (type == typeof(Guid))
                {
                    schema["format"] = "uuid";
                }
            }
            else if (type.IsEnum)
            {
                schema["type"] = "string";
                schema["enum"] = new JsonArray(Enum.GetNames(type).Select(name => JsonValue.Create(name)).ToArray<JsonNode>());
            }
            else if (type.TryGetDictionaryValueType(out var valueType))
            {
                schema["type"] = "object";

                if (rootSchema["definitions"] != null &&
                    rootSchema["definitions"].AsObject().ContainsKey(valueType!.FullName!))
                {
                    schema["additionalProperties"] = new JsonObject { ["$ref"] = $"#/definitions/{valueType.FullName}" };
                }
                else
                {
                    schema["additionalProperties"] = GenerateJsonSchema(valueType, rootSchema);
                }
            }
            else if (type.TryGetCollectionElementType(out var elementType))
            {
                schema["type"] = "array";

                if (rootSchema["definitions"] != null &&
                    rootSchema["definitions"].AsObject().ContainsKey(elementType!.FullName!))
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
                rootSchema["definitions"][type.FullName!] = new JsonObject();

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
                        rootSchema["definitions"].AsObject().ContainsKey(memberType.FullName!))
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
                            defaultValue = JsonNode.Parse(JsonSerializer.Serialize(functionPropertyAttribute.DefaultValue, options));
                            propertyInfo["default"] = defaultValue;
                        }

                        if (functionPropertyAttribute.PossibleValues is { Length: > 0 })
                        {
                            var enums = new JsonArray();

                            foreach (var value in functionPropertyAttribute.PossibleValues)
                            {
                                var @enum = JsonNode.Parse(JsonSerializer.Serialize(value, options));

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
                                enums.Add(JsonNode.Parse(defaultValue.ToJsonString(options)));
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

                schema["additionalProperties"] = false;
                rootSchema["definitions"] ??= new JsonObject();
                rootSchema["definitions"][type.FullName] = schema;
                return new JsonObject { ["$ref"] = $"#/definitions/{type.FullName}" };
            }

            return schema;
        }

        private static bool TryGetSimpleTypeSchema(this Type type, out string schemaType)
        {
            switch (type)
            {
                case not null when type == typeof(object):
                    schemaType = "object";
                    return true;
                case not null when type == typeof(bool):
                    schemaType = "boolean";
                    return true;
                case not null when type == typeof(float) ||
                                   type == typeof(double) ||
                                   type == typeof(decimal):
                    schemaType = "number";
                    return true;
                case not null when type == typeof(char) ||
                                   type == typeof(string) ||
                                   type == typeof(Guid) ||
                                   type == typeof(DateTime) ||
                                   type == typeof(DateTimeOffset):
                    schemaType = "string";
                    return true;
                case not null when type == typeof(int) ||
                                   type == typeof(long) ||
                                   type == typeof(uint) ||
                                   type == typeof(byte) ||
                                   type == typeof(sbyte) ||
                                   type == typeof(ulong) ||
                                   type == typeof(short) ||
                                   type == typeof(ushort):
                    schemaType = "integer";
                    return true;
                default:
                    schemaType = null;
                    return false;
            }
        }

        private static bool TryGetDictionaryValueType(this Type type, out Type valueType)
        {
            valueType = null;

            if (!type.IsGenericType) { return false; }

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(Dictionary<,>) ||
                genericTypeDefinition == typeof(IDictionary<,>) ||
                genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
            {
                return InternalTryGetDictionaryValueType(type, out valueType);
            }

            // Check implemented interfaces for dictionary types
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType) { continue; }

                var interfaceTypeDefinition = @interface.GetGenericTypeDefinition();

                if (interfaceTypeDefinition == typeof(IDictionary<,>) ||
                    interfaceTypeDefinition == typeof(IReadOnlyDictionary<,>))
                {
                    return InternalTryGetDictionaryValueType(@interface, out valueType);
                }
            }

            return false;

            bool InternalTryGetDictionaryValueType(Type dictType, out Type dictValueType)
            {
                dictValueType = null;
                var genericArgs = dictType.GetGenericArguments();

                // The key type is not string, which cannot be represented in JSON object property names
                if (genericArgs[0] != typeof(string))
                {
                    throw new InvalidOperationException($"Cannot generate schema for dictionary type '{dictType.FullName}' with non-string key type.");
                }

                dictValueType = genericArgs[1].UnwrapNullableType();
                return true;
            }
        }

        private static readonly Type[] arrayTypes =
        [
            typeof(IEnumerable<>),
            typeof(ICollection<>),
            typeof(IReadOnlyCollection<>),
            typeof(List<>),
            typeof(IList<>),
            typeof(IReadOnlyList<>),
            typeof(HashSet<>),
            typeof(ISet<>),
            typeof(IReadOnlySet<>)
        ];

        private static bool TryGetCollectionElementType(this Type type, out Type elementType)
        {
            elementType = null;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (!type.IsGenericType) { return false; }

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (!arrayTypes.Contains(genericTypeDefinition)) { return false; }

            elementType = type.GetGenericArguments()[0].UnwrapNullableType();
            return true;
        }

        private static Type UnwrapNullableType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;

        private static Type GetMemberType(MemberInfo member)
            => member switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw new ArgumentException($"{nameof(MemberInfo)} must be of type {nameof(FieldInfo)}, {nameof(PropertyInfo)}", nameof(member))
            };
    }
}
