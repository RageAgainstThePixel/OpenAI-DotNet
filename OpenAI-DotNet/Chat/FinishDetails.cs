// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class FinishDetails
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        public override string ToString() => Type;

        public static implicit operator string(FinishDetails details) => details?.ToString();
    }
}