// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class CodeInterpreterContainer
    {
        public CodeInterpreterContainer() { }

        public CodeInterpreterContainer(IEnumerable<string> fileIds)
        {
            FileIds = fileIds?.ToList() ?? throw new NullReferenceException(nameof(fileIds));
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "auto";

        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }
    }
}
