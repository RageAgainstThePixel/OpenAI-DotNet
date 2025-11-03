// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// A message created by an Assistant or a user.
    /// Messages can include text, images, and other files.
    /// Messages stored as a list on the Thread.
    /// </summary>
    public sealed class MessageResponse : BaseResponse, IServerSentEvent
    {
        public MessageResponse() { }

        internal MessageResponse(MessageResponse other) => AppendFrom(other);

        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MessageDelta Delta { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the message was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The thread ID that this message belongs to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The status of the message, which can be either 'in_progress', 'incomplete', or 'completed'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<MessageStatus>))]
        public MessageStatus Status { get; private set; }

        /// <summary>
        /// On an incomplete message, details about why the message is incomplete.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("incomplete_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IncompleteDetails IncompleteDetails { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the message was completed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CompletedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CompletedAt
            => CompletedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CompletedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the message was marked as incomplete.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("incomplete_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? IncompleteAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? IncompleteAt
            => IncompleteAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(IncompleteAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The entity that produced the message. One of user or assistant.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        private List<Content> content = new();

        /// <summary>
        /// The content of the message in array of text and/or images.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        public IReadOnlyList<Content> Content
        {
            get => content;
            private set => content = value?.ToList();
        }

        /// <summary>
        /// If applicable, the ID of the assistant that authored this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }

        /// <summary>
        /// If applicable, the ID of the run associated with the authoring of this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("run_id")]
        public string RunId { get; private set; }

        /// <summary>
        /// A list of files attached to the message, and the tools they were added to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attachments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<Attachment> Attachments { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        public static implicit operator string(MessageResponse message) => message?.ToString();

        public static implicit operator Message(MessageResponse response)
            => new(response.Content, response.Role, response.Attachments, response.Metadata);

        public override string ToString() => Id;

        /// <summary>
        /// Formats all of the <see cref="Content"/> items into a single string,
        /// putting each item on a new line.
        /// </summary>
        /// <returns><see cref="string"/> of all <see cref="Content"/>.</returns>
        public string PrintContent()
        {
            return Delta != null
                ? Delta.PrintContent()
                : content == null
                    ? string.Empty
                    : string.Join("\n", content.Select(c => c?.ToString()));
        }

        /// <summary>
        /// Converts the <see cref="Content"/> to the specified <see cref="JsonSchema"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to used for structured outputs.</typeparam>
        /// <param name="options"><see cref="JsonSerializerOptions"/>.</param>
        /// <returns>Deserialized <see cref="JsonSchema"/> object.</returns>
        public T FromSchema<T>(JsonSerializerOptions options = null)
        {
            options ??= OpenAIClient.JsonSerializationOptions;
            return JsonSerializer.Deserialize<T>(PrintContent(), options);
        }

        internal void AppendFrom(MessageResponse other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id))
            {
                if (Id != other.Id)
                {
                    throw new InvalidOperationException($"Attempting to append a different object than the original! {Id} != {other.Id}");
                }
            }
            else
            {
                Id = other.Id;
            }

            Object = other.Object;

            if (other.Delta != null)
            {
                if (Role == 0 && other.Delta.Role > 0)
                {
                    Role = other.Delta.Role;
                }
                else if (other.Delta.Role == 0 && Role > 0)
                {
                    other.Delta.Role = Role;
                }

                if (other.Delta.Content != null)
                {
                    content ??= new List<Content>();
                    content.AppendFrom(other.Delta.Content);
                }

                Delta = other.Delta;

                // bail early since we only care about the delta content
                return;
            }

            Delta = null;

            if (Role == 0 &&
                other.Role > 0)
            {
                Role = other.Role;
            }

            if (other.content != null)
            {
                content = other.content;
            }

            if (CreatedAtUnixTimeSeconds == 0 &&
                other.CreatedAtUnixTimeSeconds > 0)
            {
                CreatedAtUnixTimeSeconds = other.CreatedAtUnixTimeSeconds;
            }

            if (other.CompletedAtUnixTimeSeconds.HasValue)
            {
                CompletedAtUnixTimeSeconds = other.CompletedAtUnixTimeSeconds;
            }

            if (other.IncompleteAtUnixTimeSeconds.HasValue)
            {
                IncompleteAtUnixTimeSeconds = other.IncompleteAtUnixTimeSeconds;
            }

            if (other.Status > 0)
            {
                Status = other.Status;
            }

            if (other.IncompleteDetails != null)
            {
                IncompleteDetails = other.IncompleteDetails;
            }

            if (other.Attachments != null)
            {
                Attachments = other.Attachments;
            }

            if (other.Metadata != null)
            {
                Metadata = other.Metadata;
            }
        }
    }
}
