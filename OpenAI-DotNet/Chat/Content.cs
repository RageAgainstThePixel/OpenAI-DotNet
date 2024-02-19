// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Chat
{
    public sealed class Content
    {
        public Content() { }

        public Content(ImageUrl imageUrl)
        {
            Type = ContentType.ImageUrl;
            ImageUrl = imageUrl;
        }

        public Content(string input)
        {
            Type = ContentType.Text;
            Text = input;
        }

        public Content(ContentType type, string input)
        {
            Type = type;

            switch (Type)
            {
                case ContentType.Text:
                    Text = input;
                    break;
                case ContentType.ImageUrl:
                    ImageUrl = new ImageUrl(input);
                    break;
            }
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
        public ContentType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageUrl ImageUrl { get; private set; }

        public static implicit operator Content(string input) => new(ContentType.Text, input);

        public static implicit operator Content(ImageUrl imageUrl) => new(imageUrl);
    }
}