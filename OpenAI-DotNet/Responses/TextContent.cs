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

        public TextContent(string text, ResponseContentType type = ResponseContentType.InputText)
        {
            if (type != ResponseContentType.InputText && type != ResponseContentType.OutputText)
            {
                throw new ArgumentException("Invalid response content type. Must be InputText or OutputText.", nameof(type));
            }

            Type = type;
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

        private List<IAnnotation> annotations;

        /// <summary>
        /// The annotations of the text output.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<IAnnotation> Annotations
        {
            get => annotations;
            private set => annotations = value?.ToList();
        }

        private string delta;

        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Delta
        {
            get => delta;
            internal set
            {
                if (value == null)
                {
                    delta = null;
                }
                else
                {
                    delta += value;
                }
            }
        }

        /// <summary>
        /// A list of message content tokens with log probability information.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logprobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<LogProbInfo> LogProbs { get; private set; }

        [JsonIgnore]
        public string Object => Type.ToString();

        internal void InsertAnnotation(IAnnotation item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            annotations ??= [];

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
            => Delta ?? Text ?? string.Empty;
    }
}
