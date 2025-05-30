// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum CodeInterpreterOutputType
    {
        [EnumMember(Value = "logs")]
        Logs,
        [EnumMember(Value = "image")]
        Image,
        [EnumMember(Value = "files")]
        Files
    }
}
