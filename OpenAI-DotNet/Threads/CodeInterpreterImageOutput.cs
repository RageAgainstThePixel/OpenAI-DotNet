// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CodeInterpreterImageOutput
    {
        /// <summary>
        /// The file ID of the image.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }
    }
}