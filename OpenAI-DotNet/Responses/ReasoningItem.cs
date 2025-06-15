// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A description of the chain of thought used by a reasoning model while generating a response.
    /// </summary>
    public sealed class ReasoningItem : BaseResponse, IResponseItem
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; } = ResponseItemType.Reasoning;

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("status")]
        public ResponseStatus Status { get; private set; }

        private List<ReasoningSummary> summary = new();

        /// <summary>
        /// Reasoning text contents.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("summary")]
        public IReadOnlyList<ReasoningSummary> Summary
        {
            get => summary;
            private set => summary = value?.ToList() ?? new();
        }

        internal void InsertSummary(ReasoningSummary item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            summary ??= new();

            if (index > summary.Count)
            {
                for (var i = summary.Count; i < index; i++)
                {
                    summary.Add(null);
                }
            }

            summary.Insert(index, item);
        }
    }
}
