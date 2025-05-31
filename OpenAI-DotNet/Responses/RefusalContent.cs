// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class RefusalContent : IResponseContent
    {
        /// <summary>
        /// The type of the refusal. Always `refusal`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; }

        /// <summary>
        /// The refusal explanation from the model.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("refusal")]
        public string Refusal { get; private set; }
    }
}
