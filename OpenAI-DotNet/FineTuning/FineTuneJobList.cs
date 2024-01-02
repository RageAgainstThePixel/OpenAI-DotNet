// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    [Obsolete("Use ListResponse<FineTuneJobResponse>")]
    public sealed class FineTuneJobList
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<FineTuneJob> Jobs { get; private set; }

        [JsonInclude]
        [JsonPropertyName("has_more")]
        public bool HasMore { get; private set; }
    }
}