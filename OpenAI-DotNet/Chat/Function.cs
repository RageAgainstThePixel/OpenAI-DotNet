using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    /// <summary>
    /// <see href="https://platform.openai.com/docs/guides/gpt/function-calling"/>
    /// </summary>
    public class Function
    {
        internal Function(Function other) => CopyFrom(other);

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
                    arguments = JsonNode.Parse(argumentsString);
                }

                return arguments;
            }
            private set => arguments = value;
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
    }
}
