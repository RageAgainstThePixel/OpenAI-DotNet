using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Embeddings
{
    /// <summary>
    /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.<br/>
    /// <see href="https://platform.openai.com/docs/guides/embeddings"/>
    /// </summary>
    public sealed class EmbeddingsEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public EmbeddingsEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "embeddings";

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
        public async Task<EmbeddingsResponse> CreateEmbeddingAsync(string input, string model = null, string user = null)
            => await CreateEmbeddingAsync(new EmbeddingsRequest(input, model, user)).ConfigureAwait(false);

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
        public async Task<EmbeddingsResponse> CreateEmbeddingAsync(IEnumerable<string> input, string model = null, string user = null)
            => await CreateEmbeddingAsync(new EmbeddingsRequest(input, model, user)).ConfigureAwait(false);

        /// <summary>
        /// Creates an embedding vector representing the input text.
        /// </summary>
        /// <returns><see cref="EmbeddingsResponse"/></returns>
        public async Task<EmbeddingsResponse> CreateEmbeddingAsync(EmbeddingsRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request, Api.JsonSerializationOptions).ToJsonStringContent();
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return response.DeserializeResponse<EmbeddingsResponse>(responseAsString, Api.JsonSerializationOptions);
        }
    }
}
