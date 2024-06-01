// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI
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

        public Content(ImageFile imageFile)
        {
            Type = ContentType.ImageFile;
            ImageFile = imageFile;
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
                case ContentType.ImageFile:
                    throw new ArgumentException("Use the ImageFile constructor for ImageFile content.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
        public ContentType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public dynamic Text { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageUrl ImageUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_file")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageFile ImageFile { get; private set; }

        public static implicit operator Content(string input) => new(ContentType.Text, input);

        public static implicit operator Content(ImageUrl imageUrl) => new(imageUrl);

        public static implicit operator Content(ImageFile imageFile) => new(imageFile);

        public override string ToString()
            => Type switch
            {
                ContentType.Text => Text?.ToString(),
                ContentType.ImageUrl => ImageUrl.ToString(),
                ContentType.ImageFile => ImageFile.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(Type))
            };
    }
}
