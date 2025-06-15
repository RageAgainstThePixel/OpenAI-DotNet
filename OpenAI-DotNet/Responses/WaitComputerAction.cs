// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A wait action.
    /// </summary>
    public sealed class WaitComputerAction : IComputerAction
    {
        public WaitComputerAction() { }

        /// <summary>
        /// Specifies the event type.For a wait action, this property is always set to `wait`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Wait;
    }
}
