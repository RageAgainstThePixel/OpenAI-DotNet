using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class FineTuneJobResponse : BaseResponse
    {
        public static implicit operator FineTuneJob(FineTuneJobResponse jobResponse)
            => new FineTuneJob
            {
                Id = jobResponse.Id,
                Object = jobResponse.Object,
                Model = jobResponse.Model,
                CreatedAtUnixTime = jobResponse.CreatedUnixTime,
                Events = jobResponse.Events.ToList(),
                FineTunedModel = jobResponse.FineTunedModel,
                HyperParams = jobResponse.HyperParams,
                OrganizationId = jobResponse.OrganizationId,
                ResultFiles = jobResponse.ResultFiles.ToList(),
                Status = jobResponse.Status,
                ValidationFiles = jobResponse.ValidationFiles.ToList(),
                TrainingFiles = jobResponse.TrainingFiles.ToList(),
                UpdatedAtUnixTime = jobResponse.UpdatedAtUnixTime
            };

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedUnixTime { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

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