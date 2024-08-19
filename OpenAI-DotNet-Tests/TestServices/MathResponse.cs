// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Tests.StructuredOutput
{
    internal sealed class MathResponse
    {
        [JsonInclude]
        [JsonPropertyName("steps")]
        public IReadOnlyList<MathStep> Steps { get; private set; }

        [JsonInclude]
        [JsonPropertyName("final_answer")]
        public string FinalAnswer { get; private set; }
    }

    internal sealed class MathStep
    {
        [JsonInclude]
        [JsonPropertyName("explanation")]
        public string Explanation { get; private set; }

        [JsonInclude]
        [JsonPropertyName("output")]
        public string Output { get; private set; }
    }
}
