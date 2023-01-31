using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    internal sealed class FineTuneJobResponse : BaseResponse
    {
        [JsonConstructor]
        public FineTuneJobResponse(string id, string @object, string model, int createdUnixTime, IReadOnlyList<Event> events, string fineTunedModel, HyperParams hyperParams, string organizationId, IReadOnlyList<FileData> resultFiles, string status, IReadOnlyList<FileData> validationFiles, IReadOnlyList<FileData> trainingFiles, int updatedAtUnixTime)
        {
            Id = id;
            Object = @object;
            Model = model;
            CreatedUnixTime = createdUnixTime;
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
        public int CreatedUnixTime { get; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

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
        public string Status { get; }

        [JsonPropertyName("validation_files")]
        public IReadOnlyList<FileData> ValidationFiles { get; }

        [JsonPropertyName("training_files")]
        public IReadOnlyList<FileData> TrainingFiles { get; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAtUnixTime { get; }

        [JsonIgnore]
        public DateTime UpdatedAt => DateTimeOffset.FromUnixTimeSeconds(UpdatedAtUnixTime).DateTime;

        public static implicit operator FineTuneJob(FineTuneJobResponse jobResponse)
            => new FineTuneJob(
                jobResponse.Id,
                jobResponse.Object,
                jobResponse.Model,
                jobResponse.CreatedUnixTime,
                jobResponse.Events.ToList(),
                jobResponse.FineTunedModel,
                jobResponse.HyperParams,
                jobResponse.OrganizationId,
                jobResponse.ResultFiles.ToList(),
                jobResponse.Status,
                jobResponse.ValidationFiles.ToList(),
                jobResponse.TrainingFiles.ToList(),
                jobResponse.UpdatedAtUnixTime);
    }
}
