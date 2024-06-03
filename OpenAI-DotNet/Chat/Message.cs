// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Message
    {
        internal Message(Delta other) => CopyFrom(other);

        public Message() { }

        /// <summary>
        /// Creates a new message to insert into a chat conversation.
        /// </summary>
        /// <param name="role">
        /// The <see cref="OpenAI.Role"/> of the author of this message.
        /// </param>
        /// <param name="content">
        /// The contents of the message.
        /// </param>
        /// <param name="name"></param>
        public Message(Role role, IEnumerable<Content> content, string name = null)
        {
            Role = role;
            Content = content.ToList();
            Name = name;
        }

        /// <summary>
        /// Creates a new message to insert into a chat conversation.
        /// </summary>
        /// <param name="role">
        /// The <see cref="OpenAI.Role"/> of the author of this message.
        /// </param>
        /// <param name="content">
        /// The contents of the message.
        /// </param>
        /// <param name="name"></param>
        public Message(Role role, string content, string name = null)
        {
            Role = role;
            Content = content;
            Name = name;
        }

        /// <inheritdoc />
        public Message(Tool tool, string content)
            : this(Role.Tool, content, tool.Function.Name)
        {
            ToolCallId = tool.Id;
        }

        /// <summary>
        /// Creates a new message to insert into a chat conversation.
        /// </summary>
        /// <param name="tool">Tool used for message.</param>
        /// <param name="content">Tool function response.</param>
        public Message(Tool tool, IEnumerable<Content> content)
            : this(Role.Tool, content, tool.Function.Name)
        {
            ToolCallId = tool.Id;
        }

        /// <summary>
        /// Creates a new message to insert into a chat conversation.
        /// </summary>
        /// <param name="toolCallId">The tool_call_id to use for the message.</param>
        /// <param name="toolFunctionName">Name of the function call.</param>
        /// <param name="content">Tool function response.</param>
        public Message(string toolCallId, string toolFunctionName, IEnumerable<Content> content)
            : this(Role.Tool, content, toolFunctionName)
        {
            ToolCallId = toolCallId;
        }

        /// <summary>
        /// The <see cref="OpenAI.Role"/> of the author of this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        /// <summary>
        /// The contents of the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public dynamic Content { get; private set; }

        private List<Tool> toolCalls;

        /// <summary>
        /// The tool calls generated by the model, such as function calls.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_calls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<Tool> ToolCalls
        {
            get => toolCalls;
            private set => toolCalls = value.ToList();
        }

        [JsonInclude]
        [JsonPropertyName("tool_call_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ToolCallId { get; private set; }

        /// <summary>
        /// The function that should be called, as generated by the model.
        /// </summary>
        [JsonInclude]
        [Obsolete("Replaced by ToolCalls")]
        [JsonPropertyName("function_call")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Function Function { get; private set; }

        /// <summary>
        /// Optional, The name of the author of this message.<br/>
        /// May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; private set; }

        public override string ToString() => Content?.ToString() ?? string.Empty;

        public static implicit operator string(Message message) => message?.ToString();

        internal void CopyFrom(Delta other)
        {
            if (Role == 0 &&
                other?.Role > 0)
            {
                Role = other.Role;
            }

            if (other?.Content != null)
            {
                Content += other.Content;
            }

            if (!string.IsNullOrWhiteSpace(other?.Name))
            {
                Name = other.Name;
            }

            if (other is { ToolCalls: not null })
            {
                toolCalls ??= new List<Tool>();

                foreach (var otherToolCall in other.ToolCalls)
                {
                    if (otherToolCall == null) { continue; }

                    if (otherToolCall.Index.HasValue)
                    {
                        if (otherToolCall.Index + 1 > toolCalls.Count)
                        {
                            toolCalls.Insert(otherToolCall.Index.Value, new Tool(otherToolCall));
                        }

                        toolCalls[otherToolCall.Index.Value].CopyFrom(otherToolCall);
                    }
                    else
                    {
                        toolCalls.Add(new Tool(otherToolCall));
                    }
                }
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (other?.Function != null)
            {
                if (Function == null)
                {
                    Function = new Function(other.Function);
                }
                else
                {
                    Function.CopyFrom(other.Function);
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
