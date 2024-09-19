// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class TruncationStrategy
    {
        /// <summary>
        /// The truncation strategy to use for the thread.
        /// The default is 'auto'. If set to 'last_messages',
        /// the thread will be truncated to the n most recent messages in the thread. When set to 'auto',
        /// messages in the middle of the thread will be dropped to fit the context length of the model, 'max_prompt_tokens'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<TruncationStrategies>))]
        public TruncationStrategies Type { get; private set; }

        /// <summary>
        /// The number of most recent messages from the thread when constructing the context for the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_messages")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? LastMessages { get; private set; }
    }
}
