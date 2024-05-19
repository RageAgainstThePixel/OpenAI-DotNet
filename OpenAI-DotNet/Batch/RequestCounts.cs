using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public class RequestCounts
    {
        /// <summary>
        /// Total number of requests in the batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total")]
        public int Total { get; private set; }

        /// <summary>
        /// Number of requests that have been completed successfully.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed")]
        public int Completed { get; private set; }

        /// <summary>
        /// Number of requests that have failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed")]
        public int Failed { get; private set; }
    }
}