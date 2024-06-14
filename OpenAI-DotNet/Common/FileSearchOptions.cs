// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class FileSearchOptions
    {
        public FileSearchOptions() { }

        public FileSearchOptions(int maxNumberOfResults)
        {
            MaxNumberOfResults = maxNumberOfResults switch
            {
                < 1 => throw new ArgumentOutOfRangeException(nameof(maxNumberOfResults), "Max number of results must be greater than 0."),
                > 50 => throw new ArgumentOutOfRangeException(nameof(maxNumberOfResults), "Max number of results must be less than 50."),
                _ => maxNumberOfResults
            };
        }

        [JsonInclude]
        [JsonPropertyName("max_num_results")]
        public int MaxNumberOfResults { get; private set; }
    }
}
