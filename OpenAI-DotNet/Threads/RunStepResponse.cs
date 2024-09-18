// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// A detailed list of steps the Assistant took as part of a Run.
    /// An Assistant can call tools or create Messages during it's run.
    /// Examining Run Steps allows you to introspect how the Assistant is getting to it's final results.
    /// </summary>
    public sealed class RunStepResponse : BaseResponse, IServerSentEvent
    {
        public RunStepResponse() { }

        internal RunStepResponse(RunStepResponse other) => AppendFrom(other);

        /// <summary>
        /// The identifier of the run step, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public RunStepDelta Delta { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CreatedAt
            => CreatedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The ID of the assistant associated with the run step.
        /// </summary>
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
        /// The type of run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<RunStepType>))]
        public RunStepType Type { get; private set; }

        /// <summary>
        /// The status of the run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<RunStatus>))]
        public RunStatus Status { get; private set; }

        /// <summary>
        /// The details of the run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("step_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public StepDetails StepDetails { get; private set; }

        /// <summary>
        /// The last error associated with this run step. Will be null if there are no errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Error LastError { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step expired. A step is considered expired if the parent run is expired.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expired_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ExpiredAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        [Obsolete("use ExpiredAtUnixTimeSeconds")]
        public int? ExpiresAtUnitTimeSeconds => ExpiredAtUnixTimeSeconds;

        [JsonIgnore]
        public DateTime? ExpiredAt
            => ExpiredAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(ExpiredAtUnixTimeSeconds.Value).DateTime
                : null;

        [JsonIgnore]
        [Obsolete("Use ExpiredAt")]
        public DateTime? ExpiresAt => ExpiredAt;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step was cancelled.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cancelled_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CancelledAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CancelledAt
            => CancelledAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CancelledAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? FailedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? FailedAt
            => FailedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(FailedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run step completed.
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
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// Usage statistics related to the run step. This value will be `null` while the run step's status is `in_progress`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("usage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Usage Usage { get; private set; }

        public static implicit operator string(RunStepResponse runStep) => runStep?.ToString();

        public override string ToString() => Id;

        internal void AppendFrom(RunStepResponse other)
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
                if (other.Delta.StepDetails != null)
                {
                    if (StepDetails == null)
                    {
                        StepDetails = new StepDetails(other.Delta.StepDetails);
                    }
                    else
                    {
                        StepDetails.AppendFrom(other.Delta.StepDetails);
                    }
                }

                Delta = other.Delta;

                // don't update other fields if we are just appending Delta
                return;
            }

            Delta = null;

            if (other.CreatedAtUnixTimeSeconds.HasValue)
            {
                CreatedAtUnixTimeSeconds = other.CreatedAtUnixTimeSeconds;
            }

            if (other.Type > 0)
            {
                Type = other.Type;
            }

            if (other.Status > 0)
            {
                Status = other.Status;
            }

            if (other.StepDetails != null)
            {
                StepDetails = other.StepDetails;
            }

            if (other.LastError != null)
            {
                LastError = other.LastError;
            }

            if (other.ExpiredAtUnixTimeSeconds.HasValue)
            {
                ExpiredAtUnixTimeSeconds = other.ExpiredAtUnixTimeSeconds;
            }

            if (other.CancelledAtUnixTimeSeconds.HasValue)
            {
                CancelledAtUnixTimeSeconds = other.CancelledAtUnixTimeSeconds;
            }

            if (other.FailedAtUnixTimeSeconds.HasValue)
            {
                FailedAtUnixTimeSeconds = other.FailedAtUnixTimeSeconds;
            }

            if (other.CompletedAtUnixTimeSeconds.HasValue)
            {
                CompletedAtUnixTimeSeconds = other.CompletedAtUnixTimeSeconds;
            }

            if (other.Metadata is { Count: > 0 })
            {
                Metadata = new Dictionary<string, string>(other.Metadata);
            }

            if (other.Usage != null)
            {
                Usage = other.Usage;
            }
        }
    }
}
