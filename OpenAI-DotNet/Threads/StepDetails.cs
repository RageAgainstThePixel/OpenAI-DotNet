// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// The details of the run step.
    /// </summary>
    public sealed class StepDetails
    {
        public StepDetails() { }

        internal StepDetails(StepDetails other) => AppendFrom(other);

        /// <summary>
        /// Details of the message creation by the run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message_creation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public RunStepMessageCreation MessageCreation { get; private set; }

        private List<ToolCall> toolCalls;

        /// <summary>
        /// An array of tool calls the run step was involved in.
        /// These can be associated with one of three types of tools: 'code_interpreter', 'retrieval', or 'function'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_calls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<ToolCall> ToolCalls
        {
            get => toolCalls;
            private set => toolCalls = value?.ToList();
        }

        internal void AppendFrom(StepDetails other)
        {
            if (other.MessageCreation != null)
            {
                if (MessageCreation == null)
                {
                    MessageCreation = other.MessageCreation;
                }
                else
                {
                    MessageCreation.AppendFrom(other.MessageCreation);
                }
            }

            if (other.ToolCalls != null)
            {
                toolCalls ??= new List<ToolCall>();
                toolCalls.AppendFrom(other.ToolCalls);
            }
        }
    }
}
