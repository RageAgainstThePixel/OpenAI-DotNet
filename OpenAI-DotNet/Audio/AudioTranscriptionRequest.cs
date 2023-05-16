using System;
using System.IO;

namespace OpenAI.Audio
{
    public sealed class AudioTranscriptionRequest : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="audioPath">
        /// The audio file to transcribe, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.
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
        /// <param name="language">
        /// Optional, The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.<br/>
        /// Currently supported languages: Afrikaans, Arabic, Armenian, Azerbaijani, Belarusian, Bosnian, Bulgarian, Catalan,
        /// Chinese, Croatian, Czech, Danish, Dutch, English, Estonian, Finnish, French, Galician, German, Greek, Hebrew,
        /// Hindi, Hungarian, Icelandic, Indonesian, Italian, Japanese, Kannada, Kazakh, Korean, Latvian, Lithuanian,
        /// Macedonian, Malay, Marathi, Maori, Nepali, Norwegian, Persian, Polish, Portuguese, Romanian, Russian, Serbian,
        /// Slovak, Slovenian, Spanish, Swahili, Swedish, Tagalog, Tamil, Thai, Turkish, Ukrainian, Urdu, Vietnamese, and Welsh.
        /// </param>
        public AudioTranscriptionRequest(
            string audioPath,
            string model = null,
            string prompt = null,
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            int? temperature = null,
            string language = null)
            : this(File.OpenRead(audioPath), Path.GetFileName(audioPath), model, prompt, responseFormat, temperature, language)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="audio">
        /// The audio stream to transcribe.
        /// </param>
        /// <param name="audioName">
        /// The name of the audio file to transcribe.
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
        /// <param name="language">
        /// Optional, The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.<br/>
        /// Currently supported languages: Afrikaans, Arabic, Armenian, Azerbaijani, Belarusian, Bosnian, Bulgarian, Catalan,
        /// Chinese, Croatian, Czech, Danish, Dutch, English, Estonian, Finnish, French, Galician, German, Greek, Hebrew,
        /// Hindi, Hungarian, Icelandic, Indonesian, Italian, Japanese, Kannada, Kazakh, Korean, Latvian, Lithuanian,
        /// Macedonian, Malay, Marathi, Maori, Nepali, Norwegian, Persian, Polish, Portuguese, Romanian, Russian, Serbian,
        /// Slovak, Slovenian, Spanish, Swahili, Swedish, Tagalog, Tamil, Thai, Turkish, Ukrainian, Urdu, Vietnamese, and Welsh.
        /// </param>
        public AudioTranscriptionRequest(
            Stream audio,
            string audioName,
            string model = null,
            string prompt = null,
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            int? temperature = null,
            string language = null)
        {
            Audio = audio;

            if (string.IsNullOrWhiteSpace(audioName))
            {
                audioName = "audio.wav";
            }

            AudioName = audioName;

            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.Whisper1 : model;

            if (!Model.Contains("whisper"))
            {
                throw new ArgumentException($"{Model} is not supported", nameof(model));
            }

            Prompt = prompt;
            ResponseFormat = responseFormat;
            Temperature = temperature;
            Language = language;
        }

        ~AudioTranscriptionRequest() => Dispose(false);

        /// <summary>
        /// The audio file to transcribe, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.
        /// </summary>
        public Stream Audio { get; }

        /// <summary>
        /// The name of the audio file to transcribe.
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
        public int? Temperature { get; }

        /// <summary>
        /// Optional, The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.<br/>
        /// Currently supported languages: Afrikaans, Arabic, Armenian, Azerbaijani, Belarusian, Bosnian, Bulgarian, Catalan,
        /// Chinese, Croatian, Czech, Danish, Dutch, English, Estonian, Finnish, French, Galician, German, Greek, Hebrew,
        /// Hindi, Hungarian, Icelandic, Indonesian, Italian, Japanese, Kannada, Kazakh, Korean, Latvian, Lithuanian,
        /// Macedonian, Malay, Marathi, Maori, Nepali, Norwegian, Persian, Polish, Portuguese, Romanian, Russian, Serbian,
        /// Slovak, Slovenian, Spanish, Swahili, Swedish, Tagalog, Tamil, Thai, Turkish, Ukrainian, Urdu, Vietnamese, and Welsh.
        /// </summary>
        public string Language { get; }

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