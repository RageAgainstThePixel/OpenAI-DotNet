// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Numerics;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public struct Coordinate
    {
        [JsonConstructor]
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("y")]
        public int Y { get; private set; }

        public static implicit operator Vector2(Coordinate coordinate)
            => new(coordinate.X, coordinate.Y);

        public static implicit operator Coordinate(Vector2 position)
            => new((int)position.X, (int)position.Y);
    }
}
