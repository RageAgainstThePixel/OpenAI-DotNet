// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// The ranking options for the file search.
    /// <see href="https://platform.openai.com/docs/assistants/tools/file-search/customizing-file-search-settings"/>
    /// </summary>
    public sealed class RankingOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ranker">
        /// The ranker to use for the file search.
        /// If not specified will use the `auto` ranker.
        /// </param>
        /// <param name="scoreThreshold">
        /// The score threshold for the file search.
        /// All values must be a floating point number between 0 and 1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [JsonConstructor]
        public RankingOptions(string ranker = "auto", float scoreThreshold = 0f)
        {
            Ranker = ranker;
            ScoreThreshold = scoreThreshold switch
            {
                < 0 => throw new ArgumentOutOfRangeException(nameof(scoreThreshold), "Score threshold must be greater than or equal to 0."),
                > 1 => throw new ArgumentOutOfRangeException(nameof(scoreThreshold), "Score threshold must be less than or equal to 1."),
                _ => scoreThreshold
            };
        }

        /// <summary>
        /// The ranker to use for the file search.
        /// </summary>
        [JsonPropertyName("ranker")]
        public string Ranker { get; }

        /// <summary>
        /// The score threshold for the file search.
        /// </summary>
        [JsonPropertyName("score_threshold")]
        public float ScoreThreshold { get; }
    }
}
