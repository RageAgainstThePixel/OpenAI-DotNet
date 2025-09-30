// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class ReasoningContent : BaseResponse, IResponseContent
    {
        /// <summary>
        /// The type of the refusal. Always `reasoning_text`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; }

        /// <summary>
        /// The reasoning text from the model.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; internal set; }

        [JsonIgnore]
        public string Delta { get; internal set; }

        [JsonIgnore]
        public string Object => Type.ToString();

        public override string ToString()
            => string.IsNullOrWhiteSpace(Text) ? Delta : Text;
    }
}
