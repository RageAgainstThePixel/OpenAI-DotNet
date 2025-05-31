// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A screenshot action.
    /// </summary>
    public sealed class ScreenshotComputerAction : IComputerAction
    {
        public ScreenshotComputerAction() { }

        /// <summary>
        /// Specifies the event type.For a screenshot action, this property is always set to `screenshot`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Screenshot;
    }
}
