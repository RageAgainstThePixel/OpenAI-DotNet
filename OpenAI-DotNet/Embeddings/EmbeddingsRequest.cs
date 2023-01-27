using System;
using System.Text.Json.Serialization;
using OpenAI.Models;

namespace OpenAI.Embeddings
{
    public sealed class EmbeddingsRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">
        /// Input text to get embeddings for, encoded as a string or array of tokens.
        /// To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays.
        /// Each input must not exceed 8192 tokens in length.
        /// </param>
        /// <param name="model">
        /// The <see cref="OpenAI.Models.Model"/> to use.
        /// Defaults to: text-embedding-ada-002
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <exception cref="ArgumentNullException">A valid <see cref="input"/> string is a Required parameter.</exception>
        public EmbeddingsRequest(string input, Model model = null, string user = null)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input), $"Missing required {nameof(input)} parameter");
            }

            Input = input;
            Model = model ?? new Model("text-embedding-ada-002");
            User = user;
        }

        [JsonPropertyName("input")]
        public string Input { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("user")]
        public string User { get; }
    }
}