// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    public sealed class TextResponseFormatConfiguration
    {
        [Obsolete("Use new overload with TextResponseFormat instead")]
        public TextResponseFormatConfiguration(ChatResponseFormat type)
        {
            if (type == ChatResponseFormat.JsonSchema)
            {
                throw new ArgumentException("Use the constructor overload that accepts a JsonSchema object for ChatResponseFormat.JsonSchema.", nameof(type));
            }

            Type = type switch
            {
                ChatResponseFormat.Text => TextResponseFormat.Text,
                ChatResponseFormat.Json => TextResponseFormat.Json,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported response format: {type}")
            };
        }

        public TextResponseFormatConfiguration(JsonSchema schema)
        {
            Type = TextResponseFormat.JsonSchema;
            JsonSchema = schema;
        }

        public TextResponseFormatConfiguration(TextResponseFormat type)
        {
            if (type == TextResponseFormat.JsonSchema)
            {
                throw new ArgumentException("Use the constructor overload that accepts a JsonSchema object for TextResponseFormat.JsonSchema.", nameof(type));
            }

            Type = type;
        }

        public TextResponseFormat Type { get; }

        public JsonSchema JsonSchema { get; }

        public static implicit operator TextResponseFormatConfiguration(TextResponseFormat type) => new(type);

        public static implicit operator TextResponseFormat(TextResponseFormatConfiguration format) => format.Type;

#pragma warning disable CS0618 // Type or member is obsolete
        [Obsolete("Use new overload with TextResponseFormat instead")]
        public static implicit operator TextResponseFormatConfiguration(ChatResponseFormat type) => new(type);

        [Obsolete("Use new overload with TextResponseFormat instead")]
        public static implicit operator ChatResponseFormat(TextResponseFormatConfiguration format)
        {
            return format.Type switch
            {
                TextResponseFormat.Text => ChatResponseFormat.Text,
                TextResponseFormat.Json => ChatResponseFormat.Json,
                TextResponseFormat.JsonSchema => ChatResponseFormat.JsonSchema,
                _ => throw new ArgumentOutOfRangeException(nameof(format), $"Unsupported response format: {format.Type}")
            };
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
