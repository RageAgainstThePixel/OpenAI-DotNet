// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI
{
    public enum AnnotationType
    {
        [EnumMember(Value = "file_citation")]
        FileCitation,
        [EnumMember(Value = "file_path")]
        FilePath
    }
}