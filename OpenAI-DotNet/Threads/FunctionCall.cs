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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Arguments { get; private set; }

        /// <summary>
        /// The output of the function. This will be null if the outputs have not been submitted yet.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Output { get; private set; }

        internal void AppendFrom(FunctionCall other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(other.Name))
            {
                Name += other.Name;
            }

            if (!string.IsNullOrWhiteSpace(other.Arguments))
            {
                Arguments += other.Arguments;
            }

            if (!string.IsNullOrWhiteSpace(other.Output))
            {
                Output += other.Output;
            }
        }
    }
}
