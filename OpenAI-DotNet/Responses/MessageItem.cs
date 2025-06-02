// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class MessageItem : BaseResponse, IResponseItem
    {
        public MessageItem() { }

        public MessageItem(Role role, IResponseContent content)
            : this(role, [content])
        {
        }

        public MessageItem(Role role, IEnumerable<IResponseContent> content)
        {
            Type = ResponseItemType.Message;
            Role = role;
            Content = content?.ToList();
        }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; } = ResponseItemType.Message;

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

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        private List<IResponseContent> content = [];

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("content")]
        public IReadOnlyList<IResponseContent> Content
        {
            get => content;
            private set => content = value?.ToList() ?? [];
        }

        internal void AddContentItem(IResponseContent item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (index > content.Count)
            {
                for (var i = content.Count; i < index; i++)
                {
                    content.Add(null);
                }
            }

            content.Insert(index, item);
        }
    }
}
