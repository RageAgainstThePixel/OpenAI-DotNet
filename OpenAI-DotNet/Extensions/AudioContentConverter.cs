// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class AudioContentConverter : JsonConverter<AudioContent>
    {
        public override AudioContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string b64Data = null;
            string transcript = null;
            InputAudioFormat audioFormat = 0;
            ResponseContentType type = 0;

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
                        case "type":
                            Enum.TryParse(reader.GetString(), true, out type);

                            if (type is not ResponseContentType.InputAudio and not ResponseContentType.OutputAudio)
                            {
                                throw new JsonException($"Unexpected type: {type}");
                            }
                            break;
                        case "data":
                            b64Data = reader.GetString();
                            break;
                        case "format":
                            Enum.TryParse(reader.GetString(), true, out audioFormat);
                            break;
                        case "transcript":
                            transcript = reader.GetString();
                            break;
                        default:
                            throw new JsonException(propertyName);
                    }
                }
            }

            return new AudioContent(type, b64Data, audioFormat, transcript);
        }

        public override void Write(Utf8JsonWriter writer, AudioContent value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, options);
    }
}
