using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Assistants;
using OpenAI.Threads;

namespace OpenAI.ThreadRuns
{
    public sealed class CreateThreadAndRunRequest
    {
        public CreateThreadAndRunRequest(string assistantId)
        {
            AssistantId = assistantId;
        }

        /// <summary>
        /// The ID of the assistant to use to execute this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }

        /// <summary>
        /// Thread
        /// </summary>
        [JsonPropertyName("thread")]
        public ThreadForRun Thread { get; set; }

        /// <summary>
        /// The ID of the Model to be used to execute this run. If a value is provided here, it will override the model associated with the assistant. If not, the model associated with the assistant will be used.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Override the default system message of the assistant. This is useful for modifying the behavior on a per-run basis.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("instructions")]
        public string Instructions { get; set; }

        /// <summary>
        /// Override the tools the assistant can use for this run. This is useful for modifying the behavior on a per-run basis.
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

        public class ThreadForRun
        {
            /// <summary>
            /// A list of messages to start the thread with.
            /// </summary>
            [JsonPropertyName("messages")]
            public List<Message> Messages { get; set; } = new();

            /// <summary>
            /// Set of 16 key-value pairs that can be attached to an object.
            /// This can be useful for storing additional information about the object in a structured format.
            /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
            /// </summary>
            [JsonPropertyName("metadata")]
            public Dictionary<string, string> Metadata { get; set; }
        }
    }
}