// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class CompoundFilter : IFilter
    {
        public CompoundFilter(ComparisonFilter filter, CompoundFilterOperator type)
            : this([filter ?? throw new ArgumentNullException(nameof(filter))], type)
        {
        }

        [JsonConstructor]
        public CompoundFilter(IEnumerable<ComparisonFilter> filters, CompoundFilterOperator type)
        {
            Filters = filters?.ToList() ?? throw new ArgumentNullException(nameof(filters));
            Type = type;
        }

        [JsonPropertyName("filters")]
        public IReadOnlyList<ComparisonFilter> Filters { get; }

        [JsonPropertyName("type")]
        public CompoundFilterOperator Type { get; }
    }
}
