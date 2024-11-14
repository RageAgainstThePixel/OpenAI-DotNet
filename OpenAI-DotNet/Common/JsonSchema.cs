// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// <see href="https://platform.openai.com/docs/guides/structured-outputs"/><br/>
    /// <see href="https://json-schema.org/overview/what-is-jsonschema"/>
    /// </summary>
    public sealed class JsonSchema
    {
        public JsonSchema() { }

        /// <inheritdoc />
        public JsonSchema(string name, string schema, string description = null, bool strict = true)
            : this(name, JsonNode.Parse(schema), description, strict) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">
        /// The name of the response format. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64.
        /// </param>
        /// <param name="schema">
        /// The schema for the response format, described as a JSON Schema object.
        /// </param>
        /// <param name="description">
        /// A description of what the response format is for, used by the model to determine how to respond in the format.
        /// </param>
        /// <param name="strict">
        /// Whether to enable strict schema adherence when generating the output.
        /// If set to true, the model will always follow the exact schema defined in the schema field.
        /// Only a subset of JSON Schema is supported when strict is true.
        /// </param>
        public JsonSchema(string name, JsonNode schema, string description = null, bool strict = true)
        {
            Name = name;
            Description = description;
            Strict = strict;
            Schema = schema;
        }

        /// <summary>
        /// The name of the response format. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// A description of what the response format is for, used by the model to determine how to respond in the format.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description { get; private set; }

        /// <summary>
        /// Whether to enable strict schema adherence when generating the output.
        /// If set to true, the model will always follow the exact schema defined in the schema field.
        /// </summary>
        /// <remarks>
        /// Only a subset of JSON Schema is supported when strict is true.
        /// </remarks>
        [JsonInclude]
        [JsonPropertyName("strict")]
        public bool Strict { get; private set; }

        /// <summary>
        /// The schema for the response format, described as a JSON Schema object.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("schema")]
        public JsonNode Schema { get; private set; }

        public static implicit operator ResponseFormatObject(JsonSchema jsonSchema) => new(jsonSchema);

        public static implicit operator JsonSchema(Type type) => new(type.Name, type.GenerateJsonSchema());

        /// <inheritdoc />
        public override string ToString()
            => JsonSerializer.Serialize(this, ResponseExtensions.DebugJsonOptions);
    }
}
