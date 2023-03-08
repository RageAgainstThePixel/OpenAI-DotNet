using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Audio
{
    /// <summary>
    /// Speech to text.
    /// </summary>
    public sealed class AudioEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public AudioEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint()
            => $"{Api.BaseUrl}audio";

        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="transcriptionRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The transcribed text.</returns>
        public async Task<string> CreateTranscription(AudioTranscriptionRequest transcriptionRequest, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return null;
        }

        /// <summary>
        /// Translates audio into into English.
        /// </summary>
        /// <param name="translationRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The translated text.</returns>
        public async Task<string> CreateTranslation(AudioTranslationRequest translationRequest, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return null;
        }
    }
}