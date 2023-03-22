using System;
using System.Text.Json.Serialization;

namespace OpenAI.Files
{
    public sealed class FileData
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("bytes")]
        public int Size { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedUnixTime { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonInclude]
        [JsonPropertyName("filename")]
        public string FileName { get; private set; }

        [JsonInclude]
        [JsonPropertyName("purpose")]
        public string Purpose { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status { get; private set; }

        public static implicit operator string(FileData fileData) => fileData.Id;
    }
}
