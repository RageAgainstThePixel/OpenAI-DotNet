// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace OpenAI.Audio
{
    public sealed class AudioTranslationRequest : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="audioPath">
        /// The audio file to translate, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
        /// </param>
        /// <param name="model">
        /// ID of the model to use. Only whisper-1 is currently available.
        /// </param>
        /// <param name="prompt">
        /// Optional, An optional text to guide the model's style or continue a previous audio segment.<br/>
        /// The prompt should be in English.
        /// </param>
        /// <param name="responseFormat">
        /// Optional, The format of the transcript output, in one of these options: json, text, srt, verbose_json, or vtt.<br/>
        /// Defaults to json.
        /// </param>
        /// <param name="temperature">
        /// The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic. If set to 0,
        /// the model will use log probability to automatically increase the temperature until certain thresholds are hit.<br/>
        /// Defaults to 0
        /// </param>
        public AudioTranslationRequest(
            string audioPath,
            string model = null,
            string prompt = "response should be in english.",
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            float? temperature = null)
            : this(File.OpenRead(audioPath), Path.GetFileName(audioPath), model, prompt, responseFormat, temperature)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="audio">
        /// The audio file to translate, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
        /// </param>
        /// <param name="audioName">
        /// The name of the audio file to translate.
        /// </param>
        /// <param name="model">
        /// ID of the model to use. Only whisper-1 is currently available.
        /// </param>
        /// <param name="prompt">
        /// Optional, An optional text to guide the model's style or continue a previous audio segment.<br/>
        /// The prompt should be in English.
        /// </param>
        /// <param name="responseFormat">
        /// Optional, The format of the transcript output, in one of these options: json, text, srt, verbose_json, or vtt.<br/>
        /// Defaults to json.
        /// </param>
        /// <param name="temperature">
        /// The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic. If set to 0,
        /// the model will use log probability to automatically increase the temperature until certain thresholds are hit.<br/>
        /// Defaults to 0
        /// </param>
        public AudioTranslationRequest(
            Stream audio,
            string audioName,
            string model = null,
            string prompt = "response should be in english.",
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            float? temperature = null)
        {
            Audio = audio;

            if (string.IsNullOrWhiteSpace(audioName))
            {
                audioName = "audio.wav";
            }

            AudioName = audioName;
            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.Whisper1 : model;
            Prompt = prompt;
            ResponseFormat = responseFormat;
            Temperature = temperature;
        }

        ~AudioTranslationRequest() => Dispose(false);

        /// <summary>
        /// The audio file to translate, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.
        /// </summary>
        public Stream Audio { get; }

        /// <summary>
        /// The name of the audio file to translate.
        /// </summary>
        public string AudioName { get; }

        /// <summary>
        /// ID of the model to use. Only whisper-1 is currently available.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Optional, An optional text to guide the model's style or continue a previous audio segment.<br/>
        /// The prompt should be in English.
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// Optional, The format of the transcript output, in one of these options: json, text, srt, verbose_json, or vtt.<br/>
        /// Defaults to json.
        /// </summary>
        public AudioResponseFormat ResponseFormat { get; }

        /// <summary>
        /// The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic. If set to 0,
        /// the model will use log probability to automatically increase the temperature until certain thresholds are hit.<br/>
        /// Defaults to 0
        /// </summary>
        public float? Temperature { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Audio?.Close();
                Audio?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}