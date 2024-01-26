// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// An invocation of an Assistant on a Thread.
    /// The Assistant uses it's configuration and the Thread's Messages to perform tasks by calling models and tools.
    /// As part of a Run, the Assistant appends Messages to the Thread.
    /// </summary>
    public sealed class RunResponse : BaseResponse
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The thread ID that this run belongs to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The ID of the assistant used for execution of this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }

        /// <summary>
        /// The status of the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStatus>))]
        public RunStatus Status { get; private set; }

        /// <summary>
        /// Details on the action required to continue the run.
        /// Will be null if no action is required.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("required_action")]
        public RequiredAction RequiredAction { get; private set; }

        /// <summary>
        /// The Last error Associated with this run.
        /// Will be null if there are no errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        public Error LastError { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the thread was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run will expire.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expires_at")]
        public int? ExpiresAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? ExpiresAt
            => ExpiresAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was started.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("started_at")]
        public int? StartedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? StartedAt
            => StartedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(StartedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was cancelled.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cancelled_at")]
        public int? CancelledAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CancelledAt
            => CancelledAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CancelledAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_at")]
        public int? FailedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? FailedAt
            => FailedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(FailedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was completed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public int? CompletedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CompletedAt
            => CompletedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CompletedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The model that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// The instructions that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("instructions")]
        public string Instructions { get; private set; }

        /// <summary>
        /// The list of tools that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tools")]
        public IReadOnlyList<Tool> Tools { get; private set; }

        /// <summary>
        /// The list of File IDs the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// Usage statistics related to the run. This value will be `null` if the run is not in a terminal state (i.e. `in_progress`, `queued`, etc.).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("usage")]
        public Usage Usage { get; private set; }

        public static implicit operator string(RunResponse run) => run?.ToString();

        public override string ToString() => Id;
    }
}