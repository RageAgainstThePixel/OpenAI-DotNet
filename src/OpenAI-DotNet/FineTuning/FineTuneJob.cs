using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class FineTuneJob
    {
        [JsonConstructor]
        public FineTuneJob(string id, string @object, string model, int createdAtUnixTime, IReadOnlyList<Event> events, string fineTunedModel, HyperParams hyperParams, string organizationId, IReadOnlyList<FileData> resultFiles, string status, IReadOnlyList<FileData> validationFiles, IReadOnlyList<FileData> trainingFiles, int updatedAtUnixTime)
        {
            Id = id;
            Object = @object;
            Model = model;
            CreatedAtUnixTime = createdAtUnixTime;
            Events = events;
            FineTunedModel = fineTunedModel;
            HyperParams = hyperParams;
            OrganizationId = organizationId;
            ResultFiles = resultFiles;
            Status = status;
            ValidationFiles = validationFiles;
            TrainingFiles = trainingFiles;
            UpdatedAtUnixTime = updatedAtUnixTime;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTime { get; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTime).DateTime;

        [JsonPropertyName("events")]
        public IReadOnlyList<Event> Events { get; }

        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; }

        [JsonPropertyName("hyperparams")]
        public HyperParams HyperParams { get; }

        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; }

        [JsonPropertyName("result_files")]
        public IReadOnlyList<FileData> ResultFiles { get; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("validation_files")]
        public IReadOnlyList<FileData> ValidationFiles { get; }

        [JsonPropertyName("training_files")]
        public IReadOnlyList<FileData> TrainingFiles { get; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAtUnixTime { get; }

        [JsonIgnore]
        public DateTime UpdatedAt => DateTimeOffset.FromUnixTimeSeconds(UpdatedAtUnixTime).DateTime;

        public static implicit operator string(FineTuneJob job) => job.Id;
    }
}
