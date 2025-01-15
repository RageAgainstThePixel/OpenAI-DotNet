// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Chat;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class AudioOutputConverter : JsonConverter<AudioOutput>
    {
        public override AudioOutput Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string id = null;
            int? expiresAt = null;
            string b64Data = null;
            string transcript = null;
            Memory<byte> data = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "id":
                            id = reader.GetString();
                            break;
                        case "expires_at":
                            expiresAt = reader.GetInt32();
                            break;
                        case "data":
                            b64Data = reader.GetString();
                            break;
                        case "transcript":
                            transcript = reader.GetString();
                            break;
                        default:
                            throw new JsonException(propertyName);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(b64Data))
            {
                data = Convert.FromBase64String(b64Data);
            }

            return new AudioOutput(id, expiresAt, data, transcript);
        }

        public override void Write(Utf8JsonWriter writer, AudioOutput value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new { id = value.Id });
        }
    }
}
