// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// An action to type in text.
    /// </summary>
    public sealed class TypeComputerAction : IComputerAction
    {
        public TypeComputerAction() { }

        public TypeComputerAction(string text)
        {
            Type = ComputerActionType.Type;
            Text = text;
        }

        /// <summary>
        /// Specifies the event type.For a type action, this property is always set to `type`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ComputerActionType Type { get; private set; } = ComputerActionType.Type;

        /// <summary>
        /// The text to type.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("text")]
        public string Text { get; private set; }
    }
}
