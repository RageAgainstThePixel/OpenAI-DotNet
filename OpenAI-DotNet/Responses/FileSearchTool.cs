// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class FileSearchTool : ITool
    {
        public static implicit operator Tool(FileSearchTool fileSearchTool) => new(fileSearchTool as ITool);

        public FileSearchTool() { }

        public FileSearchTool(string vectorStoreId, int? maxNumberOfResults = null, RankingOptions rankingOptions = null, IEnumerable<IFilter> filters = null)
            : this([vectorStoreId], maxNumberOfResults, rankingOptions, filters)
        {
        }

        public FileSearchTool(IEnumerable<string> vectorStoreIds, int? maxNumberOfResults = null, RankingOptions rankingOptions = null, IEnumerable<IFilter> filters = null)
        {
            VectorStoreIds = vectorStoreIds?.ToList() ?? throw new NullReferenceException(nameof(vectorStoreIds));
            MaxNumberOfResults = maxNumberOfResults;
            RankingOptions = rankingOptions;
            Filters = filters?.ToList();
        }

        [JsonPropertyName("type")]
        public string Type => "file_search";

        [JsonInclude]
        [JsonPropertyName("vector_store_ids")]
        public IReadOnlyList<string> VectorStoreIds { get; private set; }

        [JsonInclude]
        [JsonPropertyName("max_num_results")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxNumberOfResults { get; private set; }

        [JsonInclude]
        [JsonPropertyName("ranking_options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RankingOptions RankingOptions { get; private set; }

        [JsonInclude]
        [JsonPropertyName("filters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<IFilter> Filters { get; private set; }
    }
}
