using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public sealed class BatchOutputFolder
    {
        /// <summary>
        /// The URL of the blob storage folder where the batch output will be written.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
