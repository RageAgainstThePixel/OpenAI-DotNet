using System;
using System.Text.Json.Serialization;

namespace OpenAI.Files
{
    public sealed class FileData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("bytes")]
        public int Size { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedUnixTime { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        public static implicit operator string(FileData fileData) => fileData.Id;
    }
}