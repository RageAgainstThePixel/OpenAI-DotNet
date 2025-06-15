// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A drag action.
    /// </summary>
    public sealed class DragComputerAction : IComputerAction
    {
        public DragComputerAction() { }

        public DragComputerAction(IEnumerable<Coordinate> path)
        {
            Type = ComputerActionType.Drag;
            Path = path?.ToList() ?? throw new ArgumentNullException(nameof(path), "Path cannot be null.");
        }

        /// <summary>
        /// Specifies the event type. For a drag action, this property is always set to `drag`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Drag;

        /// <summary>
        /// An array of coordinates representing the path of the drag action.
        /// A series of x/y coordinate pairs in the drag path.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public IReadOnlyList<Coordinate> Path { get; private set; }
    }
}
