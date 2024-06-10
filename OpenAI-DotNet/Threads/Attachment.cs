// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class Attachment
    {
        public Attachment() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileId">The ID of the file to attach to the message.</param>
        /// <param name="tool">The tool to add this file to.</param>
        public Attachment(string fileId, Tool tool) : this(fileId, new[] { tool }) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileId">The ID of the file to attach to the message.</param>
        /// <param name="tools">The tools to add this file to.</param>
        public Attachment(string fileId, IEnumerable<Tool> tools)
        {
            FileId = fileId;
            Tools = tools?.ToList();
        }

        /// <summary>
        /// The ID of the file to attach to the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FileId { get; private set; }

        /// <summary>
        /// The tools to add this file to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<Tool> Tools { get; private set; }
    }
}
