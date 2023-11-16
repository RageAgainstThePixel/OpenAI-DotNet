using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Threads
{
    /// <summary>
    /// Represents a step in execution of a run.
    /// </summary>
    public sealed class RunStep : BaseResponse
    {
        public static implicit operator string(RunStep step) => step?.Id;

        /// <summary>
        /// The identifier of the run step, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always `thread.run.step`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was created.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The ID of the assistant associated with the run step.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }
    
        /// <summary>
        /// The ID of the thread that was run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }
        
        /// <summary>
        /// The ID of the run that this run step is a part of.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("run_id")]
        public string RunId { get; private set; }
        
        /// <summary>
        /// The type of run step, which can be either message_creation or tool_calls.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStepType>))]
        public RunStepType Type { get; private set; }
        
        /// <summary>
        /// The status of the run step, which can be either in_progress, cancelled, failed, completed, or expired.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStepStatus>))]
        public RunStepStatus Status { get; private set; }
        
        /// <summary>
        /// The details of the run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("step_details")]
        public StepDetails StepDetails { get; private set; }

        /// <summary>
        /// The last error associated with this run step. Will be null if there are no errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        public RunLastError LastError { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step expired. A step is considered expired if the parent run is expired.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("expires_at")]
        public int? ExpiresAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was cancelled.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("cancelled_at")]
        public int? CancelledAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step failed.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("failed_at")]
        public int? FailedAt { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step completed.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public int? CompletedAt { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }
    }
}