using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Audio;

public interface IAudioEndpoint
{
    /// <summary>
    /// Transcribes audio into the input language.
    /// </summary>
    /// <param name="request"><see cref="AudioTranscriptionRequest"/>.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>The transcribed text.</returns>
    Task<string> CreateTranscriptionAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Translates audio into English.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The translated text.</returns>
    Task<string> CreateTranslationAsync(AudioTranslationRequest request, CancellationToken cancellationToken = default);
}