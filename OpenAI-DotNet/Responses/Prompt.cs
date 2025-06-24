// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class Prompt
    {
        [JsonConstructor]
        public Prompt(string id, IReadOnlyDictionary<string, object> variables = null, string version = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Variables = variables;
            Version = version;
        }

        /// <summary>
        /// The unique identifier of the prompt template to use.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Optional map of values to substitute in for variables in your prompt.
        /// The substitution values can either be strings, or other Response input types like images or files.
        /// </summary>
        [JsonPropertyName("variables")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, object> Variables { get; private set; }

        /// <summary>
        /// Optional version of the prompt template.
        /// </summary>
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Version { get; private set; }
    }
}
