using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public sealed class BatchOutputFolder
    {
        public BatchOutputFolder(string url)
        {
            Url = url;
        }

        /// <summary>
        /// The URL of the blob storage folder where the batch output will be written.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        public static implicit operator BatchOutputFolder(string url) => new(url);
    }
}
