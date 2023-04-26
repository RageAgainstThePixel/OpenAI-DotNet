using OpenAI.Extensions;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Audio
{
    /// <summary>
    /// Transforms audio into text.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/audio"/>
    /// </summary>
    public sealed class AudioEndpoint : BaseEndPoint
    {
        private class AudioResponse
        {
            public AudioResponse(string text)
            {
                Text = text;
            }

            [JsonPropertyName("text")]
            public string Text { get; }
        }

        /// <inheritdoc />
        public AudioEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "audio";

        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="request"><see cref="AudioTranscriptionRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The transcribed text.</returns>
        public async Task<string> CreateTranscriptionAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent();
            using var audioData = new MemoryStream();
            await request.Audio.CopyToAsync(audioData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(audioData.ToArray()), "file", request.AudioName);
            content.Add(new StringContent(request.Model), "model");

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                content.Add(new StringContent(request.Prompt), "prompt");
            }

            var responseFormat = request.ResponseFormat;
            content.Add(new StringContent(responseFormat.ToString().ToLower()), "response_format");

            if (request.Temperature.HasValue)
            {
                content.Add(new StringContent(request.Temperature.ToString()), "temperature");
            }

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                content.Add(new StringContent(request.Language), "language");
            }

            request.Dispose();

            var response = await Api.Client.PostAsync(GetUrl("/transcriptions"), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return responseFormat == AudioResponseFormat.Json
                ? JsonSerializer.Deserialize<AudioResponse>(responseAsString)?.Text
                : responseAsString;
        }

        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The translated text.</returns>
        public async Task<string> CreateTranslationAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent();
            using var audioData = new MemoryStream();
            await request.Audio.CopyToAsync(audioData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(audioData.ToArray()), "file", request.AudioName);
            content.Add(new StringContent(request.Model), "model");

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                content.Add(new StringContent(request.Prompt), "prompt");
            }

            var responseFormat = request.ResponseFormat;
            content.Add(new StringContent(responseFormat.ToString().ToLower()), "response_format");

            if (request.Temperature.HasValue)
            {
                content.Add(new StringContent(request.Temperature.ToString()), "temperature");
            }

            request.Dispose();

            var response = await Api.Client.PostAsync(GetUrl("/translations"), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return responseFormat == AudioResponseFormat.Json
                ? JsonSerializer.Deserialize<AudioResponse>(responseAsString)?.Text
                : responseAsString;
        }
    }
}