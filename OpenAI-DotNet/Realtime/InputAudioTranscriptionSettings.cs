// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class InputAudioTranscriptionSettings
    {
        public InputAudioTranscriptionSettings() { }

        public InputAudioTranscriptionSettings(Model model)
        {
            Model = string.IsNullOrWhiteSpace(model.Id) ? "whisper-1" : model;
        }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }
    }
}
