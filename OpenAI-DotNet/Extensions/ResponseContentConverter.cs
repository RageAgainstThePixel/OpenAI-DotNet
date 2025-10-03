// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class ResponseContentConverter : JsonConverter<IResponseContent>
    {
        public override void Write(Utf8JsonWriter writer, IResponseContent value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override IResponseContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;

            return type switch
            {
                "input_text" => root.Deserialize<TextContent>(options),
                "output_text" => root.Deserialize<TextContent>(options),
                "input_audio" => root.Deserialize<AudioContent>(options),
                "output_audio" => root.Deserialize<AudioContent>(options),
                "input_image" => root.Deserialize<ImageContent>(options),
                "input_file" => root.Deserialize<FileContent>(options),
                "refusal" => root.Deserialize<RefusalContent>(options),
                "reasoning_text" => root.Deserialize<ReasoningContent>(options),
                _ => throw new NotImplementedException($"Unknown response content type: {type}")
            };
        }
    }
}
