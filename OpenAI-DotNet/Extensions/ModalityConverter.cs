// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class ModalityConverter : JsonConverter<Modality>
    {
        public override void Write(Utf8JsonWriter writer, Modality value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            if (value.HasFlag(Modality.Text)) { writer.WriteStringValue("text"); }
            if (value.HasFlag(Modality.Audio)) { writer.WriteStringValue("audio"); }
            writer.WriteEndArray();
        }

        public override Modality Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var modalityArray = JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray();
            var modality = Modality.None;
            foreach (var modalityString in modalityArray)
            {
                modality |= modalityString.GetString() switch
                {
                    "text" => Modality.Text,
                    "audio" => Modality.Audio,
                    _ => throw new NotImplementedException($"Unknown modality: {modalityString}")
                };
            }
            return modality;
        }
    }
}
