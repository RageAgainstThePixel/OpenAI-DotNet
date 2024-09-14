// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class FileSearchOptions
    {
        public FileSearchOptions() { }

        public FileSearchOptions(int? maxNumberOfResults, RankingOptions rankingOptions = null)
        {
            MaxNumberOfResults = maxNumberOfResults switch
            {
                null => null,
                < 1 => throw new ArgumentOutOfRangeException(nameof(maxNumberOfResults), "Max number of results must be greater than 0."),
                > 50 => throw new ArgumentOutOfRangeException(nameof(maxNumberOfResults), "Max number of results must be less than 50."),
                _ => maxNumberOfResults
            };
            RankingOptions = rankingOptions ?? new RankingOptions();
        }

        [JsonInclude]
        [JsonPropertyName("max_num_results")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? MaxNumberOfResults { get; private set; }

        [JsonInclude]
        [JsonPropertyName("ranking_options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public RankingOptions RankingOptions { get; private set; }
    }
}
