// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum ComputerActionType
    {
        [EnumMember(Value = "click")]
        Click = 1,
        [EnumMember(Value = "double_click")]
        DoubleClick,
        [EnumMember(Value = "drag")]
        Drag,
        [EnumMember(Value = "keypress")]
        KeyPress,
        [EnumMember(Value = "move")]
        Move,
        [EnumMember(Value = "screenshot")]
        Screenshot,
        [EnumMember(Value = "scroll")]
        Scroll,
        [EnumMember(Value = "type")]
        Type,
        [EnumMember(Value = "wait")]
        Wait
    }
}
