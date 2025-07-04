// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class InputAudioTranscriptionSettings
    {
        public InputAudioTranscriptionSettings() { }

        public InputAudioTranscriptionSettings(Model model, string prompt = null, string language = null)
        {
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.Whisper1 : model;
            Prompt = prompt;
            Language = language;
        }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prompt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Prompt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("language")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Language { get; private set; }
    }
}
