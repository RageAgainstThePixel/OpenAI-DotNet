// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ConversationItem
    {
        public ConversationItem() { }

        public ConversationItem(Role role, IEnumerable<RealtimeContent> content)
        {
            Role = role;
            Type = ConversationItemType.Message;
            Content = content?.ToList() ?? new List<RealtimeContent>();

            if (role is not (Role.Assistant or Role.User))
            {
                throw new ArgumentException("Role must be either 'user' or 'assistant'.");
            }

            if (role == Role.User && !Content.All(c => c.Type is RealtimeContentType.InputAudio or RealtimeContentType.InputText))
            {
                throw new ArgumentException("User messages must contain only input text or input audio content.");
            }

            if (role == Role.Assistant && !Content.All(c => c.Type is RealtimeContentType.Text or RealtimeContentType.Audio))
            {
                throw new ArgumentException("Assistant messages must contain only text or audio content.");
            }
        }

        public ConversationItem(Role role, RealtimeContent content)
            : this(role, new[] { content })
        {
        }

        public ConversationItem(RealtimeContent content)
            : this(Role.User, new[] { content })
        {
        }

        public ConversationItem(ToolCall toolCall, string output)
        {
            Type = ConversationItemType.FunctionCallOutput;
            FunctionCallId = toolCall.Id;
            FunctionOutput = output;
        }

        public ConversationItem(Tool tool)
        {
            Type = ConversationItemType.FunctionCall;
            FunctionName = tool.Function.Name;
        }

        public static implicit operator ConversationItem(string text) => new(text);

        /// <summary>
        /// The unique ID of the item.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, must be "realtime.item".
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The type of the item ("message", "function_call", "function_call_output").
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ConversationItemType Type { get; private set; }

        /// <summary>
        /// The status of the item ("completed", "in_progress", "incomplete").
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public RealtimeResponseStatus Status { get; private set; }

        /// <summary>
        /// The role associated with the item ("user", "assistant", "system").
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        /// <summary>
        /// The content of the item.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        public IReadOnlyList<RealtimeContent> Content { get; private set; }

        /// <summary>
        /// The ID of the function call (for "function_call" items).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("call_id")]
        public string FunctionCallId { get; private set; }

        /// <summary>
        /// The name of the function being called.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string FunctionName { get; private set; }

        /// <summary>
        /// The arguments of the function call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        public JsonNode FunctionArguments { get; private set; }

        /// <summary>
        /// The output of the function call (for "function_call_output" items).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output")]
        public string FunctionOutput { get; private set; }
    }
}
