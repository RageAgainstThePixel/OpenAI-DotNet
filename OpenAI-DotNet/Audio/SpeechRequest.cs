// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System;
using System.Text.Json.Serialization;

namespace OpenAI.Audio
{
    public sealed class SpeechRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">The text to generate audio for. The maximum length is 4096 characters.</param>
        /// <param name="model">One of the available TTS models. Defaults to tts-1.</param>
        /// <param name="voice">The voice to use when generating the audio.</param>
        /// <param name="responseFormat">The format to audio in. Supported formats are mp3, opus, aac, flac, wav and pcm.</param>
        /// <param name="speed">The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.</param>
        public SpeechRequest(string input, Model model = null, Voice voice = null, SpeechResponseFormat responseFormat = SpeechResponseFormat.MP3, float? speed = null)
        {
            Input = !string.IsNullOrWhiteSpace(input) ? input : throw new ArgumentException("Input cannot be null or empty.", nameof(input));
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.TTS_1 : model;
            Voice = string.IsNullOrWhiteSpace(voice?.Id) ? OpenAI.Voice.Alloy : voice;
            ResponseFormat = responseFormat;
            Speed = speed;
        }

        /// <summary>
        /// One of the available TTS models. Defaults to tts-1.
        /// </summary>
        [JsonPropertyName("model")]
        [FunctionProperty("One of the available TTS models. Defaults to tts-1.", true, "tts-1", "tts-1-hd")]
        public string Model { get; }

        /// <summary>
        /// The text to generate audio for. The maximum length is 4096 characters.
        /// </summary>
        [JsonPropertyName("input")]
        [FunctionProperty("The text to generate audio for. The maximum length is 4096 characters.", true)]
        public string Input { get; }

        /// <summary>
        /// The voice to use when generating the audio.
        /// </summary>
        [JsonPropertyName("voice")]
        [FunctionProperty("The voice to use when generating the audio.", true, "alloy", "echo", "fable", "onyx", "nova", "shimmer")]
        public string Voice { get; }

        /// <summary>
        /// The format to audio in. Supported formats are mp3, opus, aac, flac, wav and pcm.
        /// </summary>
        [JsonPropertyName("response_format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<SpeechResponseFormat>))]
        [FunctionProperty("The format to audio in. Supported formats are mp3, opus, aac, flac, wav and pcm.", false, SpeechResponseFormat.MP3)]
        public SpeechResponseFormat ResponseFormat { get; }

        /// <summary>
        /// The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.
        /// </summary>
        [JsonPropertyName("speed")]
        [FunctionProperty("The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.", false, 1.0f)]
        public float? Speed { get; }
    }
}
