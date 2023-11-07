using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class FineTuneJob
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
        public int? CreatedAtUnixTime { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTime ?? 0).DateTime;

        [JsonInclude]
        [JsonPropertyName("finished_at")]
        public int? FinishedAtUnixTime { get; private set; }

        [JsonIgnore]
        public DateTime FinishedAt => DateTimeOffset.FromUnixTimeSeconds(FinishedAtUnixTime ?? 0).DateTime;

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

        public static implicit operator string(FineTuneJob job) => job.Id;
    }
}
