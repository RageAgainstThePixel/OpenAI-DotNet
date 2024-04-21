// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Audio
{
    /// <summary>
    /// Transforms audio into text.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/audio"/>
    /// </summary>
    public sealed class AudioEndpoint : OpenAIBaseEndpoint
    {
        /// <inheritdoc />
        public AudioEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "audio";

        /// <summary>
        /// Generates audio from the input text.
        /// </summary>
        /// <param name="request"><see cref="SpeechRequest"/>.</param>
        /// <param name="chunkCallback">Optional, partial chunk <see cref="ReadOnlyMemory{T}"/> callback to stream audio as it arrives.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ReadOnlyMemory{T}"/></returns>
        public async Task<ReadOnlyMemory<byte>> CreateSpeechAsync(SpeechRequest request, Func<ReadOnlyMemory<byte>, Task> chunkCallback = null, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("/speech"), jsonContent, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, jsonContent, null, cancellationToken).ConfigureAwait(false);
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using var memoryStream = new MemoryStream();
            int bytesRead;
            var totalBytesRead = 0;
            var buffer = new byte[8192];

            while ((bytesRead = await responseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                await memoryStream.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwait(false);

                if (chunkCallback != null)
                {
                    try
                    {
                        await chunkCallback(new ReadOnlyMemory<byte>(memoryStream.GetBuffer(), totalBytesRead, bytesRead)).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                totalBytesRead += bytesRead;
            }

            await response.CheckResponseAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return new ReadOnlyMemory<byte>(memoryStream.GetBuffer(), 0, totalBytesRead);
        }

        [Obsolete("Use CreateTranscriptionTextAsync or CreateTranscriptionJsonAsync instead.")]
        public async Task<string> CreateTranscriptionAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default)
            => await CreateTranscriptionTextAsync(request, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="request"><see cref="AudioTranscriptionRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The transcribed text.</returns>
        public async Task<string> CreateTranscriptionTextAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default)
        {
            var responseAsString = await Internal_CreateTranscriptionAsync(request, cancellationToken).ConfigureAwait(false);
            return request.ResponseFormat is AudioResponseFormat.Json or AudioResponseFormat.Verbose_Json
                ? JsonSerializer.Deserialize<AudioResponse>(responseAsString)?.Text
                : responseAsString;
        }

        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <remarks>This method expects the request format to be either <see cref="AudioResponseFormat.Json"/> or <see cref="AudioResponseFormat.Verbose_Json"/>.</remarks>
        /// <param name="request"><see cref="AudioTranscriptionRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AudioResponse"/>.</returns>
        public async Task<AudioResponse> CreateTranscriptionJsonAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ResponseFormat is not (AudioResponseFormat.Json or AudioResponseFormat.Verbose_Json))
            {
                throw new ArgumentException("Response format must be Json or Verbose Json.", nameof(request.ResponseFormat));
            }

            var responseAsString = await Internal_CreateTranscriptionAsync(request, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<AudioResponse>(responseAsString);
        }

        private async Task<string> Internal_CreateTranscriptionAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent();
            using var audioData = new MemoryStream();
            await request.Audio.CopyToAsync(audioData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(audioData.ToArray()), "file", request.AudioName);
            content.Add(new StringContent(request.Model), "model");

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                content.Add(new StringContent(request.Language), "language");
            }

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                content.Add(new StringContent(request.Prompt), "prompt");
            }

            content.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

            if (request.Temperature.HasValue)
            {
                content.Add(new StringContent(request.Temperature.Value.ToString(CultureInfo.InvariantCulture)), "temperature");
            }

            switch (request.TimestampGranularities)
            {
                case TimestampGranularity.Segment:
                case TimestampGranularity.Word:
                    content.Add(new StringContent(request.TimestampGranularities.ToString().ToLower()), "timestamp_granularities[]");
                    break;
            }

            request.Dispose();

            using var response = await client.Client.PostAsync(GetUrl("/transcriptions"), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, content, null, cancellationToken).ConfigureAwait(false);
            return responseAsString;
        }

        [Obsolete("Use CreateTranslationTextAsync or CreateTranslationJsonAsync instead.")]
        public async Task<string> CreateTranslationAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default)
            => await CreateTranslationTextAsync(request, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The translated text.</returns>
        public async Task<string> CreateTranslationTextAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default)
        {
            var responseAsString = await Internal_CreateTranslationAsync(request, cancellationToken).ConfigureAwait(false);
            return request.ResponseFormat is AudioResponseFormat.Json or AudioResponseFormat.Verbose_Json
                ? JsonSerializer.Deserialize<AudioResponse>(responseAsString)?.Text
                : responseAsString;
        }

        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AudioResponse> CreateTranslationJsonAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ResponseFormat is not (AudioResponseFormat.Json or AudioResponseFormat.Verbose_Json))
            {
                throw new ArgumentException("Response format must be Json or Verbose Json.", nameof(request.ResponseFormat));
            }

            var responseAsString = await Internal_CreateTranslationAsync(request, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<AudioResponse>(responseAsString);
        }

        private async Task<string> Internal_CreateTranslationAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default)
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

            content.Add(new StringContent(request.ResponseFormat.ToString().ToLower()), "response_format");

            if (request.Temperature.HasValue)
            {
                content.Add(new StringContent(request.Temperature.Value.ToString(CultureInfo.InvariantCulture)), "temperature");
            }

            request.Dispose();

            using var response = await client.Client.PostAsync(GetUrl("/translations"), content, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, content, null, cancellationToken).ConfigureAwait(false);
            return responseAsString;
        }
    }
}