// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI
{
    public enum AnnotationType
    {
        [EnumMember(Value = "file_citation")]
        FileCitation = 1,
        [EnumMember(Value = "file_path")]
        FilePath,
        [EnumMember(Value = "url_citation")]
        UrlCitation,
        [EnumMember(Value = "container_file_citation")]
        ContainerFileCitation
    }
}
