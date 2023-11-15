using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAI.Extensions;
using OpenAI.ThreadRuns;

namespace OpenAI.ThreadRuns
{
    /// <summary>
    /// Represents a step in execution of a run.
    /// </summary>
    public sealed class RunStep
    {
        public static implicit operator string(RunStep step) => step?.Id;

        /// <summary>
        /// The identifier of the run step, which can be referenced in API endpoints.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The object type, which is always `thread.run.step`.
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was created.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; set; }

        /// <summary>
        /// The ID of the assistant associated with the run step.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }
    
        /// <summary>
        /// The ID of the thread that was run.
        /// </summary>
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; set; }
        
        /// <summary>
        /// The ID of the run that this run step is a part of.
        /// </summary>
        [JsonPropertyName("run_id")]
        public string RunId { get; set; }
        
        /// <summary>
        /// The type of run step, which can be either message_creation or tool_calls.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStepType>))]
        public RunStepType Type { get; set; }
        
        /// <summary>
        /// The status of the run step, which can be either in_progress, cancelled, failed, completed, or expired.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<RunStepStatus>))]
        public RunStepStatus Status { get; set; }
        
        /// <summary>
        /// The details of the run step.
        /// </summary>
        [JsonPropertyName("step_details")]
        public StepDetails StepDetails { get; set; }

        /// <summary>
        /// The last error associated with this run step. Will be null if there are no errors.
        /// </summary>
        [JsonPropertyName("last_error")]
        public RunLastError LastError { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step expired. A step is considered expired if the parent run is expired.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("expires_at")]
        public int? ExpiresAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was cancelled.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("cancelled_at")]
        public int? CancelledAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step failed.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("failed_at")]
        public int? FailedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step completed.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("completed_at")]
        public int? CompletedAt { get; set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; set; }
    }
}