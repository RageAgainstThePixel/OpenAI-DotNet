using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Assistants;

namespace OpenAI.Threads
{
    public sealed class CreateThreadRunRequest
    {
        public CreateThreadRunRequest(string assistantId)
        {
            AssistantId = assistantId;
        }
    
        /// <summary>
        /// The ID of the assistant used for execution of this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }
    
        /// <summary>
        /// The model that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The instructions that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("instructions")]
        public string Instructions { get; set; }
    
        /// <summary>
        /// The list of tools that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("tools")]
        public AssistantTool[] Tools { get; set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}