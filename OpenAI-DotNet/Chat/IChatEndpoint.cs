using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat;

public interface IChatEndpoint
{
    /// <summary>
    /// Creates a completion for the chat message
    /// </summary>
    /// <param name="chatRequest">The chat request which contains the message content.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="ChatResponse"/>.</returns>
    Task<ChatResponse> GetCompletionAsync(ChatRequest chatRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
    /// </summary>
    /// <param name="chatRequest">The chat request which contains the message content.</param>
    /// <param name="resultHandler">An action to be called as each new result arrives.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="ChatResponse"/>.</returns>
    /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
    Task<ChatResponse> StreamCompletionAsync(ChatRequest chatRequest, Action<ChatResponse> resultHandler,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Created a completion for the chat message and stream the results as they come in.<br/>
    /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
    /// you may need to use <see cref="StreamCompletionAsync(ChatRequest, Action{ChatResponse}, CancellationToken)"/> instead.
    /// </summary>
    /// <param name="chatRequest">The chat request which contains the message content.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="ChatResponse"/>.</returns>
    /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
    IAsyncEnumerable<ChatResponse> StreamCompletionEnumerableAsync(ChatRequest chatRequest,
        CancellationToken cancellationToken = default);
}