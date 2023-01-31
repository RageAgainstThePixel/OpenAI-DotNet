using System;
using System.Text.Json.Serialization;

namespace OpenAI.Files
{
    public sealed class FileData
    {
        [JsonConstructor]
        public FileData(string id, string @object, int size, int createdUnixTime, string fileName, string purpose, string status)
        {
            Id = id;
            Object = @object;
            Size = size;
            CreatedUnixTime = createdUnixTime;
            FileName = fileName;
            Purpose = purpose;
            Status = status;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("bytes")]
        public int Size { get; }

        [JsonPropertyName("created_at")]
        public int CreatedUnixTime { get; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonPropertyName("filename")]
        public string FileName { get; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; }

        [JsonPropertyName("status")]
        public string Status { get; }

        public static implicit operator string(FileData fileData) => fileData.Id;
    }
}
