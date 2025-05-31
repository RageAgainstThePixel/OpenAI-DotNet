// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A collection of keypresses the model would like to perform.
    /// </summary>
    public sealed class KeyPressComputerAction : IComputerAction
    {
        public KeyPressComputerAction() { }

        public KeyPressComputerAction(IEnumerable<string> keys)
        {
            Type = ComputerActionType.KeyPress;
            Keys = keys?.ToList() ?? throw new ArgumentNullException(nameof(keys), "Keys cannot be null.");
        }

        /// <summary>
        /// Specifies the event type. For a keypress action, this property is always set to `keypress`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ComputerActionType Type { get; private set; } = ComputerActionType.KeyPress;

        /// <summary>
        /// The combination of keys the model is requesting to be pressed.
        /// This is an array of strings, each representing a key.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("keys")]
        public IReadOnlyList<string> Keys { get; private set; }
    }
}
