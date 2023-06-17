using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    /// <summary>
    /// <see href="https://platform.openai.com/docs/guides/gpt/function-calling"/>
    /// </summary>
    public class Function
    {
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
        /// An optional JSON object describing the parameters of the function that the model should generate in JSON schema format (json-schema.org).
        /// </param>
        /// <param name="arguments">
        /// The arguments to use when calling the function.
        /// </param>
        public Function(string name, string description = null, JsonNode parameters = null, JsonNode arguments = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
            }

            if (name.Length > 64)
            {
                throw new ArgumentException($"{nameof(name)} cannot be longer than 64 characters.", nameof(name));
            }

            Name = name;
            Description = description;
            Parameters = parameters;
            Arguments = arguments;
        }

        /// <summary>
        /// The name of the function to generate arguments for.<br/>
        /// May contain a-z, A-Z, 0-9, and underscores and dashes, with a maximum length of 64 characters.
        /// Recommended to not begin with a number or a dash.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; private set; }

        /// <summary>
        /// The optional description of the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; private set; }

        /// <summary>
        /// The optional parameters of the function.
        /// Describe the parameters that the model should generate in JSON schema format (json-schema.org).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parameters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public JsonNode Parameters { get; private set; }

        /// <summary>
        /// The arguments to use when calling the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public JsonNode Arguments { get; private set; }
    }
}
