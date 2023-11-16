using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Assistants
{
    public sealed class AssistantRequest
    {
        public AssistantRequest(string model = null, string name = null, string description = null, string instructions = null, IReadOnlyList<Tool> tools = null, IReadOnlyList<string> fileIds = null, IReadOnlyDictionary<string, object> metadata = null)
        {
            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.GPT3_5_Turbo : model;
            Name = name;
            Description = description;
            Instructions = instructions;
            Tools = tools;
            FileIds = fileIds;
            Metadata = metadata;
        }

        /// <summary>
        /// ID of the model to use.
        /// You can use the List models API to see all of your available models,
        /// or see our Model overview for descriptions of them.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// The name of the assistant.
        /// The maximum length is 256 characters.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// The description of the assistant.
        /// The maximum length is 512 characters.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        /// The system instructions that the assistant uses.
        /// The maximum length is 32768 characters.
        /// </summary>
        [JsonPropertyName("instructions")]
        public string Instructions { get; }

        /// <summary>
        /// A list of tool enabled on the assistant.
        /// There can be a maximum of 128 tools per assistant.
        /// Tools can be of types 'code_interpreter', 'retrieval', or 'function'.
        /// </summary>
        [JsonPropertyName("tools")]
        public IReadOnlyList<Tool> Tools { get; }

        /// <summary>
        /// A list of file IDs attached to this assistant.
        /// There can be a maximum of 20 files attached to the assistant.
        /// Files are ordered by their creation date in ascending order.
        /// </summary>
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, object> Metadata { get; }
    }
}