// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    [Obsolete("use FineTuneJobResponse")]
    public sealed class FineTuneJob : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int? CreateAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        [Obsolete("Use CreateAtUnixTimeSeconds")]
        public int? CreatedAtUnixTime => CreateAtUnixTimeSeconds;

        [JsonIgnore]
        public DateTime? CreatedAt
            => CreateAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CreateAtUnixTimeSeconds.Value).DateTime
                : null;

        [JsonInclude]
        [JsonPropertyName("finished_at")]
        public int? FinishedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        [Obsolete("Use FinishedAtUnixTimeSeconds")]
        public int? FinishedAtUnixTime => CreateAtUnixTimeSeconds;

        [JsonIgnore]
        public DateTime? FinishedAt
            => FinishedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(FinishedAtUnixTimeSeconds.Value).DateTime
                : null;

        [JsonInclude]
        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; private set; }

        [JsonInclude]
        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("result_files")]
        public IReadOnlyList<string> ResultFiles { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        public JobStatus Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("validation_file")]
        public string ValidationFile { get; private set; }

        [JsonInclude]
        [JsonPropertyName("training_file")]
        public string TrainingFile { get; private set; }

        [JsonInclude]
        [JsonPropertyName("hyperparameters")]
        public HyperParams HyperParameters { get; private set; }

        [JsonInclude]
        [JsonPropertyName("trained_tokens")]
        public int? TrainedTokens { get; private set; }

        [JsonIgnore]
        public IReadOnlyList<Event> Events { get; internal set; } = new List<Event>();

        public static implicit operator FineTuneJobResponse(FineTuneJob job) => new(job);

        public static implicit operator string(FineTuneJob job) => job?.ToString();

        public override string ToString() => Id;
    }
}
