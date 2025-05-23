// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace OpenAI.Audio
{
    public sealed class AudioTranscriptionRequest : IDisposable
    {
        [Obsolete("Use new .ctr overload with chunkingStrategy and include")]
        public AudioTranscriptionRequest(
            string audioPath,
            string model,
            string prompt,
            AudioResponseFormat responseFormat,
            float? temperature,
            string language,
            TimestampGranularity timestampGranularity)
            : this(
                audio: File.OpenRead(audioPath),
                audioName: Path.GetFileName(audioPath),
                model: model,
                chunkingStrategy: null,
                include: null,
                language: language,
                prompt: prompt,
                responseFormat: responseFormat,
                temperature: temperature,
                timestampGranularity: timestampGranularity)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="audioPath">
        /// The audio file to transcribe, in one of these formats flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
        /// </param>
        /// <param name="model">
        /// ID of the model to use.
        /// </param>
        /// <param name="chunkingStrategy">
        /// Controls how the audio is cut into chunks. When set to "auto",
        /// the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries.
        /// server_vad object can be provided to tweak VAD detection parameters manually.
        /// If unset, the audio is transcribed as a single block.
        /// </param>
        /// <param name="include">
        /// Additional information to include in the transcription response.
        /// logprobs will return the log probabilities of the tokens in the response to understand the model's confidence in the transcription.
        /// logprobs only works with response_format set to json and only with the models gpt-4o-transcribe and gpt-4o-mini-transcribe.
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
        /// <param name="timestampGranularity">
        /// The timestamp granularities to populate for this transcription.
        /// response_format must be set verbose_json to use timestamp granularities.
        /// Either or both of these options are supported: <see cref="TimestampGranularity.Word"/>, or <see cref="TimestampGranularity.Segment"/>. <br/>
        /// Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
        /// </param>
        public AudioTranscriptionRequest(
            string audioPath,
            string model = null,
            ChunkingStrategy chunkingStrategy = null,
            string[] include = null,
            string language = null,
            string prompt = null,
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            float? temperature = null,
            TimestampGranularity timestampGranularity = TimestampGranularity.None)
            : this(
                audio: File.OpenRead(audioPath),
                audioName: Path.GetFileName(audioPath),
                model: model,
                chunkingStrategy: chunkingStrategy,
                include: include,
                language: language,
                prompt: prompt,
                responseFormat: responseFormat,
                temperature: temperature,
                timestampGranularity: timestampGranularity)
        {
        }

        [Obsolete("Use new .ctr overload with chunkingStrategy and include")]
        public AudioTranscriptionRequest(
            Stream audio,
            string audioName,
            string model,
            string prompt,
            AudioResponseFormat responseFormat,
            float? temperature,
            string language,
            TimestampGranularity timestampGranularity)
            : this(
                audio: audio,
                audioName: audioName,
                model: model,
                chunkingStrategy: null,
                include: null,
                language: language,
                prompt: prompt,
                responseFormat: responseFormat,
                temperature: temperature,
                timestampGranularity: timestampGranularity)
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
        /// <param name="chunkingStrategy">
        /// Controls how the audio is cut into chunks. When set to "auto",
        /// the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries.
        /// server_vad object can be provided to tweak VAD detection parameters manually.
        /// If unset, the audio is transcribed as a single block.
        /// </param>
        /// <param name="include">
        /// Additional information to include in the transcription response.
        /// logprobs will return the log probabilities of the tokens in the response to understand the model's confidence in the transcription.
        /// logprobs only works with response_format set to json and only with the models gpt-4o-transcribe and gpt-4o-mini-transcribe.
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
        /// <param name="timestampGranularity">
        /// The timestamp granularities to populate for this transcription.
        /// response_format must be set verbose_json to use timestamp granularities.
        /// Either or both of these options are supported: <see cref="TimestampGranularity.Word"/>, or <see cref="TimestampGranularity.Segment"/>. <br/>
        /// Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
        /// </param>
        public AudioTranscriptionRequest(
            Stream audio,
            string audioName,
            string model = null,
            ChunkingStrategy chunkingStrategy = null,
            string[] include = null,
            string language = null,
            string prompt = null,
            AudioResponseFormat responseFormat = AudioResponseFormat.Json,
            float? temperature = null,
            TimestampGranularity timestampGranularity = TimestampGranularity.None)
        {
            Audio = audio;

            if (string.IsNullOrWhiteSpace(audioName))
            {
                audioName = "audio.wav";
            }

            AudioName = audioName;
            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.Whisper1 : model;
            ChunkingStrategy = chunkingStrategy;

            if (include != null && responseFormat is not (AudioResponseFormat.Json or AudioResponseFormat.Verbose_Json))
            {
                throw new ArgumentException($"{nameof(responseFormat)} must be set {AudioResponseFormat.Json} or {AudioResponseFormat.Verbose_Json} to use include.");
            }

            Include = include;
            Language = language;
            Prompt = prompt;
            ResponseFormat = responseFormat;
            Temperature = temperature;

            if (timestampGranularity != TimestampGranularity.None && responseFormat != AudioResponseFormat.Verbose_Json)
            {
                throw new ArgumentException($"{nameof(responseFormat)} must be set {AudioResponseFormat.Verbose_Json} to use timestamp granularities.");
            }

            TimestampGranularities = timestampGranularity;
        }

        ~AudioTranscriptionRequest() => Dispose(false);

        /// <summary>
        /// The audio file to transcribe, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
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
        /// Controls how the audio is cut into chunks.
        /// When set to "auto", the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries.
        /// server_vad object can be provided to tweak VAD detection parameters manually.
        /// If unset, the audio is transcribed as a single block.
        /// </summary>
        public ChunkingStrategy ChunkingStrategy { get; }

        /// <summary>
        /// Additional information to include in the transcription response.
        /// logprobs will return the log probabilities of the tokens in the response to understand the model's confidence in the transcription.
        /// logprobs only works with response_format set to json and only with the models gpt-4o-transcribe and gpt-4o-mini-transcribe.
        /// </summary>
        public string[] Include { get; }

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

        /// <summary>
        /// The timestamp granularities to populate for this transcription.
        /// response_format must be set verbose_json to use timestamp granularities.
        /// Either or both of these options are supported: <see cref="TimestampGranularity.Word"/>, or <see cref="TimestampGranularity.Segment"/>. <br/>
        /// Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
        /// </summary>
        public TimestampGranularity TimestampGranularities { get; }

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
