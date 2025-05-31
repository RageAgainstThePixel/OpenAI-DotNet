// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A click action.
    /// </summary>
    public sealed class ClickComputerAction : IComputerAction
    {
        public ClickComputerAction() { }

        public ClickComputerAction(ComputerClickButtonType button, Coordinate position)
        {
            Type = ComputerActionType.Click;
            Button = button;
            X = position.X;
            Y = position.Y;
        }

        /// <summary>
        /// Specifies the event type. For a click action, this property is always set to `click`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Click;

        /// <summary>
        /// Indicates which mouse button was pressed during the click.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("button")]
        public ComputerClickButtonType Button { get; private set; }

        /// <summary>
        /// The x-coordinate where the click occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        /// <summary>
        /// The y-coordinate where the click occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("y")]
        public int Y { get; private set; }
    }
}
