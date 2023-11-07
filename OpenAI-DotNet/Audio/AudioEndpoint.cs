using System;
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
        /// Generates audio from the input text.
        /// </summary>
        /// <param name="request"><see cref="SpeechRequest"/>.</param>
        /// <param name="chunkCallback">Optional, partial chunk <see cref="ReadOnlyMemory{T}"/> callback to stream audio as it arrives.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ReadOnlyMemory{T}"/></returns>
        public async Task<ReadOnlyMemory<byte>> CreateSpeechAsync(SpeechRequest request, Func<ReadOnlyMemory<byte>, Task> chunkCallback = null, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl("/speech"), jsonContent, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(cancellationToken).ConfigureAwait(false);
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

            return new ReadOnlyMemory<byte>(memoryStream.GetBuffer(), 0, totalBytesRead);
        }

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
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);

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
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);

            return responseFormat == AudioResponseFormat.Json
                ? JsonSerializer.Deserialize<AudioResponse>(responseAsString)?.Text
                : responseAsString;
        }
    }
}