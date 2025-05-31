// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class TextContent : IResponseContent
    {
        public TextContent() { }

        public TextContent(string text)
        {
            Type = ResponseContentType.InputText;
            Text = text;
        }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("annotations")]
        public IReadOnlyList<Annotation> Annotations { get; private set; }

        public static implicit operator TextContent(string input) => new(input);
    }
}
