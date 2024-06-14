// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Content : IAppendable<Content>
    {
        public Content() { }

        public Content(string text)
            : this(ContentType.Text, text)
        {
        }

        public Content(TextContent textContent)
        {
            Type = ContentType.Text;
            Text = textContent;
        }

        public Content(ImageUrl imageUrl)
        {
            Type = ContentType.ImageUrl;
            ImageUrl = imageUrl;
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
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ContentType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonConverter(typeof(StringOrObjectConverter<TextContent>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object Text { get; private set; }

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
                ContentType.ImageUrl => ImageUrl?.ToString(),
                ContentType.ImageFile => ImageFile?.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(Type))
            } ?? string.Empty;

        public void AppendFrom(Content other)
        {
            if (other == null) { return; }

            if (other.Type > 0)
            {
                Type = other.Type;
            }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (other.Text is TextContent otherTextContent)
            {
                if (Text is TextContent textContent)
                {
                    textContent.AppendFrom(otherTextContent);
                }
                else
                {
                    Text = otherTextContent;
                }
            }
            else if (other.Text is string otherStringContent)
            {
                if (!string.IsNullOrWhiteSpace(otherStringContent))
                {
                    Text += otherStringContent;
                }
            }

            if (other.ImageUrl != null)
            {
                if (ImageUrl == null)
                {
                    ImageUrl = other.ImageUrl;
                }
                else
                {
                    ImageUrl.AppendFrom(other.ImageUrl);
                }
            }

            if (other.ImageFile != null)
            {
                if (ImageFile == null)
                {
                    ImageFile = other.ImageFile;
                }
                else
                {
                    ImageFile.AppendFrom(other.ImageFile);
                }
            }
        }
    }
}
