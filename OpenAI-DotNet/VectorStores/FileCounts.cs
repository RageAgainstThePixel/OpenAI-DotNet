// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    public sealed class FileCounts
    {
        /// <summary>
        /// The number of files that are currently being processed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("in_progress")]
        public int InProgress { get; private set; }

        /// <summary>
        /// The number of files that have been successfully processed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed")]
        public int Completed { get; private set; }

        /// <summary>
        /// The number of files that have failed to process.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed")]
        public int Failed { get; private set; }

        /// <summary>
        /// The number of files that were cancelled.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cancelled")]
        public int Cancelled { get; private set; }

        /// <summary>
        /// The total number of files.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total")]
        public int Total { get; private set; }
    }
}
