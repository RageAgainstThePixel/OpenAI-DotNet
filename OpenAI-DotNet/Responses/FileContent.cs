// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class FileContent : BaseResponse, IResponseContent
    {
        public FileContent() { }

        public FileContent(string fileName, string fileData)
        {
            Type = ResponseContentType.InputFile;
            FileData = fileData;
            FileName = fileName;
        }

        /// <summary>
        /// If the fileId starts with "http" or "https", it is a file url, otherwise it is a file id.
        /// </summary>
        /// <param name="fileId">The id or url of the file.</param>
        public FileContent(string fileId)
        {
            Type = ResponseContentType.InputFile;
            if (fileId.StartsWith("http")) {
                FileUrl = fileId;
            } else {
                FileId = fileId;
            }
        }

        public FileContent(byte[] fileData, string fileName)
        {
            Type = ResponseContentType.InputFile;
            FileData = System.Convert.ToBase64String(fileData);
            FileName = fileName;
        }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("file_data")]
        public string FileData { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("file_name")]
        public string FileName { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("file_url")]
        public string FileUrl { get; private set; }

        [JsonIgnore]
        public string Object => Type.ToString();
    }
}
