// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class ComputerScreenShot
    {
        /// <summary>
        /// Specifies the event type. For a computer screenshot, this property is always set to `computer_screenshot`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The URL of the screenshot image.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; private set; }

        /// <summary>
        /// The identifier of an uploaded file that contains the screenshot.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }
    }
}
