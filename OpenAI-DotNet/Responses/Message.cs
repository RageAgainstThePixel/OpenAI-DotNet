// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class Message : BaseResponse, IResponseItem
    {
        public static implicit operator Message(string input) => new(Role.User, input);

        public Message() { }

        public Message(Role role, string text)
            : this(role, new TextContent(text, role == Role.Assistant ? ResponseContentType.OutputText : ResponseContentType.InputText))
        {
        }

        public Message(Role role, IResponseContent content)
            : this(role, [content])
        {
        }

        public Message(Role role, IEnumerable<IResponseContent> content)
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

        internal void AddOrUpdateContentItem(IResponseContent item, int index)
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

        public override string ToString()
            => Content?.LastOrDefault()?.ToString() ?? string.Empty;

        public T FromSchema<T>(JsonSerializerOptions options = null)
        {
            options ??= OpenAIClient.JsonSerializationOptions;
            return JsonSerializer.Deserialize<T>(ToString(), options);
        }
    }
}
