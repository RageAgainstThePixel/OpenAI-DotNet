// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// A set of resources to be used by Assistants and Threads.
    /// The resources are specific to the type of tool.
    /// For example, the <see cref="Tool.CodeInterpreter"/> requres a list of file ids,
    /// While the <see cref="Tool.FileSearch"/> requires a list vector store ids.
    /// </summary>
    public sealed class ToolResources
    {
        public ToolResources() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileSearch"><see cref="FileSearchResources"/>.</param>
        /// <param name="codeInterpreter"><see cref="CodeInterpreterResources"/>.</param>
        public ToolResources(FileSearchResources fileSearch = null, CodeInterpreterResources codeInterpreter = null)
            : this(codeInterpreter, fileSearch)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="codeInterpreter"><see cref="CodeInterpreterResources"/>.</param>
        /// <param name="fileSearch"><see cref="FileSearchResources"/>.</param>
        public ToolResources(CodeInterpreterResources codeInterpreter = null, FileSearchResources fileSearch = null)
        {
            CodeInterpreter = codeInterpreter;
            FileSearch = fileSearch;
        }

        [JsonInclude]
        [JsonPropertyName("code_interpreter")]
        public CodeInterpreterResources CodeInterpreter { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_search")]
        public FileSearchResources FileSearch { get; private set; }

        public static implicit operator ToolResources(FileSearchResources fileSearch) => new(fileSearch);

        public static implicit operator ToolResources(CodeInterpreterResources codeInterpreter) => new(codeInterpreter);

        public static implicit operator ToolResources(VectorStoreRequest vectorStoreRequest) => new(new FileSearchResources(vectorStoreRequest));
    }
}
