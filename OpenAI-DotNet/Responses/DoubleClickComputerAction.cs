// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A double click action.
    /// </summary>
    public sealed class DoubleClickComputerAction : IComputerAction
    {
        public DoubleClickComputerAction() { }

        public DoubleClickComputerAction(Coordinate position)
        {
            Type = ComputerActionType.DoubleClick;
            X = position.X;
            Y = position.Y;
        }

        /// <summary>
        /// Specifies the event type.For a double click action, this property is always set to `double_click`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.DoubleClick;

        /// <summary>
        /// The x-coordinate where the double click occurred.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        /// <summary>
        /// The y-coordinate where the double click occurred.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("y")]
        public int Y { get; private set; }
    }
}
