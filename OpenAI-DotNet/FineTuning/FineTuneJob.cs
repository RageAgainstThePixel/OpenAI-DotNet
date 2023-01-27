using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class FineTuneJob
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTime { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTime).DateTime;

        [JsonPropertyName("events")]
        public IReadOnlyList<Event> Events { get; set; }

        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; set; }

        [JsonPropertyName("hyperparams")]
        public HyperParams HyperParams { get; set; }

        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("result_files")]
        public IReadOnlyList<FileData> ResultFiles { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("validation_files")]
        public IReadOnlyList<FileData> ValidationFiles { get; set; }

        [JsonPropertyName("training_files")]
        public IReadOnlyList<FileData> TrainingFiles { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAtUnixTime { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt => DateTimeOffset.FromUnixTimeSeconds(UpdatedAtUnixTime).DateTime;
    }
}