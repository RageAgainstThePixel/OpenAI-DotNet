using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ImageFile
    {
        [JsonInclude]
        [JsonPropertyName("file_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FileId { get; private set; }
    }
}