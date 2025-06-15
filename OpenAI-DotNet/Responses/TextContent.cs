// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class TextContent : BaseResponse, IResponseContent
    {
        public static implicit operator TextContent(string value) => new(value);

        public TextContent() { }

        public TextContent(string text)
        {
            Type = ResponseContentType.InputText;
            Text = text;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ResponseContentType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; internal set; }

        private List<Annotation> annotations;

        [JsonInclude]
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<Annotation> Annotations
        {
            get => annotations;
            private set => annotations = value?.ToList();
        }

        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Delta { get; internal set; }

        [JsonIgnore]
        public string Object => Type.ToString();

        internal void InsertAnnotation(Annotation item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            annotations ??= new();

            if (index > annotations.Count)
            {
                for (var i = annotations.Count; i < index; i++)
                {
                    annotations.Add(null);
                }
            }

            annotations.Insert(index, item);
        }

        public override string ToString()
            => Text;
    }
}
