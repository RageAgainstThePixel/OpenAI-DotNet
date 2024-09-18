// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public sealed class BatchResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The OpenAI API endpoint used by the batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; private set; }

        /// <summary>
        /// Errors that occured during the batch job.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("errors")]
        public BatchErrors BatchErrors { get; private set; }

        /// <summary>
        /// The ID of the input file for the batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input_file_id")]
        public string InputFileId { get; private set; }

        /// <summary>
        /// The time frame within which the batch should be processed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completion_window")]
        public string CompletionWindow { get; private set; }

        /// <summary>
        /// The current status of the batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<BatchStatus>))]
        public BatchStatus Status { get; private set; }

        /// <summary>
        /// The ID of the file containing the outputs of successfully executed requests.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_file_id")]
        public string OutputFileId { get; private set; }

        /// <summary>
        /// The ID of the file containing the outputs of requests with errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_file_id")]
        public string ErrorFileId { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch started processing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("in_progress_at")]
        public int? InProgressAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? InProgressAt
            => InProgressAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(InProgressAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch will expire.
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
        /// The Unix timestamp (in seconds) for when the batch started finalizing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("finalizing_at")]
        public int? FinalizingAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? FinalizingAt
            => FinalizingAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(FinalizingAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was completed.
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
        /// The Unix timestamp (in seconds) for when the batch failed.
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
        /// The Unix timestamp (in seconds) for when the batch expired.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expired_at")]
        public int? ExpiredAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? ExpiredAt
            => ExpiredAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(ExpiredAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was cancelled.
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
        /// The request counts for different statuses within the batch.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_counts")]
        public RequestCounts RequestCounts { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, object> Metadata { get; private set; }

        public override string ToString() => Id;

        public static implicit operator string(BatchResponse response) => response?.ToString();
    }
}
