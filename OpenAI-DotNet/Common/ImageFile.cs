// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ImageFile
    {
        /// <summary>
        /// The file ID of the image.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FileId { get; private set; }
    }
}