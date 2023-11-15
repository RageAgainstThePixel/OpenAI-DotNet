using System.Runtime.Serialization;

namespace OpenAI.Chat;

public enum AnnotationType
{
    [EnumMember(Value = "file_citation")]
    FileCitation,
    [EnumMember(Value = "file_path")]
    FilePath
}