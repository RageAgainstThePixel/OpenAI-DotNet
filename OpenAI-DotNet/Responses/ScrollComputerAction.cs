// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A scroll action.
    /// </summary>
    public sealed class ScrollComputerAction : IComputerAction
    {
        public ScrollComputerAction() { }

        public ScrollComputerAction(Coordinate position, Coordinate scrollDistance)
        {
            Type = ComputerActionType.Scroll;
            X = position.X;
            Y = position.Y;
            ScrollX = scrollDistance.X;
            ScrollY = scrollDistance.Y;
        }

        /// <summary>
        /// Specifies the event type.For a scroll action, this property is always set to `scroll`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Scroll;

        /// <summary>
        /// The x-coordinate where the scroll occurred.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        /// <summary>
        /// The y-coordinate where the scroll occurred.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("y")]
        public int Y { get; private set; }

        /// <summary>
        /// The horizontal scroll distance.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("scroll_x")]
        public int ScrollX { get; private set; }

        /// <summary>
        /// The vertical scroll distance.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("scroll_y")]
        public int ScrollY { get; private set; }
    }
}
