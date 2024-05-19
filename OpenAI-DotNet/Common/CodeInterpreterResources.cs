// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// <see cref="Tool.CodeInterpreter"/> resources.
    /// </summary>
    public sealed class CodeInterpreterResources
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileIds">
        /// A list of file IDs made available to the <see cref="Tool.CodeInterpreter"/> tool.
        /// There can be a maximum of 20 files associated with the tool.
        /// </param>
        public CodeInterpreterResources(IReadOnlyList<string> fileIds)
        {
            FileIds = fileIds;
        }

        /// <summary>
        /// A list of file IDs made available to the <see cref="Tool.CodeInterpreter"/> tool.
        /// There can be a maximum of 20 files associated with the tool.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }

        public static implicit operator CodeInterpreterResources(List<string> fileIds) => new(fileIds);
    }
}
