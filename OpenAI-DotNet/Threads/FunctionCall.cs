// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class FunctionCall
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

        /// <summary>
        /// The output of the function. This will be null if the outputs have not been submitted yet.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output")]
        public string Output { get; private set; }
    }
}
