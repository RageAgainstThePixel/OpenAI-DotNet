// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class AnnotationConverter : JsonConverter<IAnnotation>
    {
        public override void Write(Utf8JsonWriter writer, IAnnotation value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override IAnnotation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;

            return type switch
            {
                "file_citation" => root.Deserialize<FileCitation>(options),
                "file_path" => root.Deserialize<FilePath>(options),
                "url_citation" => root.Deserialize<UrlCitation>(options),
                "container_file_citation" => root.Deserialize<ContainerFileCitation>(options),
                _ => throw new NotImplementedException($"Unknown annotation type: {type}")
            };
        }
    }
}
