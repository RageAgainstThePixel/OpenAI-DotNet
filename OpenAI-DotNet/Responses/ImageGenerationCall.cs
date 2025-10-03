// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// An image generation request made by the model.
    /// </summary>
    public sealed class ImageGenerationCall : BaseResponse, IResponseItem
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; } = ResponseItemType.FunctionCall;

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("status")]
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// The generated image encoded in base64.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("result")]
        public string Result { get; private set; }

        [JsonInclude]
        [JsonPropertyName("partial_image_b64")]
        public string PartialImageResult { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("output_format")]
        public string OutputFormat { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("revised_prompt")]
        public string RevisedPrompt { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("background")]
        public string Background { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("size")]
        public string Size { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("quality")]
        public string Quality { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("partial_image_index")]
        public int? PartialImageIndex { get; internal set; }
    }
}
