using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class ToolCallFunction
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }
    
        /// <summary>
        /// The arguments that the model expects you to pass to the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        public string Arguments { get; private set; }
    }
}