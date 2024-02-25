// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// <param name="dimensions">
        /// The number of dimensions the resulting output embeddings should have.
        /// Only supported in text-embedding-3 and later models
        /// </param>
        public EmbeddingsRequest(string input, string model = null, string user = null, int? dimensions = null)
            : this(new List<string> { input }, model, user, dimensions)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input), $"Missing required {nameof(input)} parameter");
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
        /// The model id to use.<br/>
        /// Defaults to: <see cref="Model.Embedding_Ada_002"/>
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        /// <param name="dimensions">
        /// The number of dimensions the resulting output embeddings should have.
        /// Only supported in text-embedding-3 and later models
        /// </param>
        public EmbeddingsRequest(IEnumerable<string> input, string model = null, string user = null, int? dimensions = null)
        {
            Input = input?.ToList();

            if (Input?.Count == 0)
            {
                throw new ArgumentNullException(nameof(input), $"Missing required {nameof(input)} parameter");
            }

            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.Embedding_Ada_002 : model;
            User = user;
            Dimensions = dimensions;
        }

        [JsonPropertyName("input")]
        public IReadOnlyList<string> Input { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; }

        [JsonPropertyName("user")]
        public string User { get; }
    }
}
