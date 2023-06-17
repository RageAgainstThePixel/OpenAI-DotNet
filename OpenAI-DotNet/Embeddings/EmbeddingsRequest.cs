using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
        /// ID of the model to use.<br/>
        /// Defaults to: <see cref="Model.Embedding_Ada_002"/>
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <exception cref="ArgumentNullException">A valid <see cref="input"/> string is a Required parameter.</exception>
        public EmbeddingsRequest(string input, string model = null, string user = null)
            : this(new List<string> { input }, model, user)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">
        /// Input text to get embeddings for, encoded as a string or array of tokens.
        /// To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays.
        /// Each input must not exceed 8192 tokens in length.
        /// </param>
        /// <param name="model">
        /// The model id to use.
        /// Defaults to: <see cref="Model.Embedding_Ada_002"/>
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <exception cref="ArgumentNullException">A valid <see cref="input"/> string is a Required parameter.</exception>
        public EmbeddingsRequest(IEnumerable<string> input, string model = null, string user = null)
        {
            Input = input?.ToList();

            if (Input?.Count == 0)
            {
                throw new ArgumentNullException(nameof(input), $"Missing required {nameof(input)} parameter");
            }

            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.Embedding_Ada_002 : model;

            if (!Model.Contains("embedding") &&
                !Model.Contains("search") &&
                !Model.Contains("similarity"))
            {
                throw new ArgumentException($"{Model} is not supported", nameof(model));
            }

            User = user;
        }

        [JsonPropertyName("input")]
        public IReadOnlyList<string> Input { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("user")]
        public string User { get; }
    }
}
