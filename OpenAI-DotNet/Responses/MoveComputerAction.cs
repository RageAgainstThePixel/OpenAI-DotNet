// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A mouse move action.
    /// </summary>
    public sealed class MoveComputerAction : IComputerAction
    {
        public MoveComputerAction() { }

        public MoveComputerAction(Coordinate position)
        {
            Type = ComputerActionType.Move;
            X = position.X;
            Y = position.Y;
        }

        /// <summary>
        /// Specifies the event type. For a move action, this property is always set to `move`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Move;

        /// <summary>
        /// The x-coordinate to move to.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        /// <summary>
        /// The y-coordinate to move to.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("y")]
        public int Y { get; private set; }
    }
}
