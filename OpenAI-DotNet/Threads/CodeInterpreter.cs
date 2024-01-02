// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CodeInterpreter
    {
        /// <summary>
        /// The input to the Code Interpreter tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input")]
        public string Input { get; private set; }

        /// <summary>
        /// The outputs from the Code Interpreter tool call.
        /// Code Interpreter can output one or more items, including text (logs) or images (image).
        /// Each of these are represented by a different object type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("outputs")]
        public IReadOnlyList<CodeInterpreterOutputs> Outputs { get; private set; }
    }
}