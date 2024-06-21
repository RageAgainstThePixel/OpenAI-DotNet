// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Input { get; private set; }

        private List<CodeInterpreterOutputs> outputs;

        /// <summary>
        /// The outputs from the Code Interpreter tool call.
        /// Code Interpreter can output one or more items, including text (logs) or images (image).
        /// Each of these are represented by a different object type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("outputs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<CodeInterpreterOutputs> Outputs
        {
            get => outputs;
            private set => outputs = value?.ToList();
        }

        internal void AppendFrom(CodeInterpreter other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(other.Input))
            {
                Input += other.Input;
            }

            if (other.Outputs != null)
            {
                outputs ??= new List<CodeInterpreterOutputs>();
                outputs.AddRange(other.Outputs);
            }
        }
    }
}
