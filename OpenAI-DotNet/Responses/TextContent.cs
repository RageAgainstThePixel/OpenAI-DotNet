// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class TextContent : BaseResponse, IResponseContent
    {
        public static implicit operator TextContent(string input) => new(input);

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
        public string Text { get; internal set; }

        [JsonIgnore]
        public string Delta { get; internal set; }

        private List<Annotation> annotations = [];

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("annotations")]
        public IReadOnlyList<Annotation> Annotations
        {
            get => annotations;
            private set => annotations = value?.ToList() ?? [];
        }

        [JsonIgnore]
        public string Object => Type.ToString();

        internal void InsertAnnotation(Annotation item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (index > annotations.Count)
            {
                for (var i = annotations.Count; i < index; i++)
                {
                    annotations.Add(null);
                }
            }

            annotations.Insert(index, item);
        }
    }
}
