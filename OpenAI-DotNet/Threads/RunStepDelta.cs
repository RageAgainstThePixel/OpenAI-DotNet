// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public class RunStepDelta
    {
        [JsonInclude]
        [JsonPropertyName("step_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public StepDetails StepDetails { get; private set; }
    }
}
