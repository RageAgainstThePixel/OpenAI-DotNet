using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ImageFile
    {
        [JsonConstructor]
        public ImageFile(string fileId)
        {
            FileId = fileId;
        }

        [JsonInclude]
        [JsonPropertyName("file_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FileId { get; private set; }
    }
}