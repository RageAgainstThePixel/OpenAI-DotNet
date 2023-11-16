using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Assistants;
using OpenAI.Extensions;

namespace OpenAI.Threads
{
    public sealed class ThreadRun
    {
        public static implicit operator string(ThreadRun run) => run?.Id;
        
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always run.
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the thread was created.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The thread ID that this run belongs to.
        /// </summary>
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The ID of the assistant used for execution of this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }

        /// <summary>
        /// The status of the run, which can be either queued, in_progress, requires_action, cancelling, cancelled, failed, completed, or expired.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStatus>))]
        public RunStatus Status { get; private set; }

        /// <summary>
        /// Details on the action required to continue the run.Will be null if no action is required.
        /// </summary>
        [JsonPropertyName("required_action")]
        public ThreadRunRequiredAction RequiredAction { get; private set; }

        /// <summary>
        /// The Last error Associated with this run. Will Be null if there Are no errors.
        /// </summary>
        [JsonPropertyName("last_error")]
        public RunLastError LastError { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run will expire.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("expires_at")]
        public int? ExpiresAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was started.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("started_at")]
        public int? StartedAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was cancelled.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("cancelled_at")]
        public int? CancelledAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run failed.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("failed_at")]
        public int? FailedAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was completed.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("completed_at")]
        public int? CompletedAt { get; private set; }

        /// <summary>
        /// The model that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// The instructions that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("instructions")]
        public string Instructions { get; private set; }

        /// <summary>
        /// The list of tools that the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("tools")]
        public AssistantTool[] Tools { get; private set; }

        /// <summary>
        /// The list of File IDs the assistant used for this run.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("file_ids")]
        public string[] FileIds { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }
    }
}