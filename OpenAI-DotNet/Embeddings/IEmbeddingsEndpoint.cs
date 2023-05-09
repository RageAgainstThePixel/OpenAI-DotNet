using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Models;

namespace OpenAI.Embeddings;

public interface IEmbeddingsEndpoint
{
    /// <summary>
    /// Creates an embedding vector representing the input text.
    /// </summary>
    /// <param name="input">
    /// Input text to get embeddings for, encoded as a string or array of tokens.
    /// To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays.
    /// Each input must not exceed 8192 tokens in length.
    /// </param>
    /// <param name="model">
    /// ID of the model to use.
    /// Defaults to: text-embedding-ada-002
    /// </param>
    /// <param name="user">
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    /// </param>
    /// <returns><see cref="EmbeddingsResponse"/></returns>
    Task<EmbeddingsResponse> CreateEmbeddingAsync(string input, Model model = null, string user = null);

    /// <summary>
    /// Creates an embedding vector representing the input text.
    /// </summary>
    /// <param name="input">
    /// Input text to get embeddings for, encoded as a string or array of tokens.
    /// To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays.
    /// Each input must not exceed 8192 tokens in length.
    /// </param>
    /// <param name="model">
    /// ID of the model to use.
    /// Defaults to: text-embedding-ada-002
    /// </param>
    /// <param name="user">
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    /// </param>
    /// <returns><see cref="EmbeddingsResponse"/></returns>
    Task<EmbeddingsResponse> CreateEmbeddingAsync(IEnumerable<string> input, Model model = null, string user = null);

    /// <summary>
    /// Creates an embedding vector representing the input text.
    /// </summary>
    /// <returns><see cref="EmbeddingsResponse"/></returns>
    Task<EmbeddingsResponse> CreateEmbeddingAsync(EmbeddingsRequest request);
}