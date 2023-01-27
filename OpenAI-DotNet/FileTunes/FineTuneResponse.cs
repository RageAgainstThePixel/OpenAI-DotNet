using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.FileTunes
{
    public sealed class FineTuneResponse : BaseResponse
    {
        public static implicit operator FineTuneJob(FineTuneResponse response)
            => new FineTuneJob
            {
                Id = response.Id,
                Object = response.Object,
                Model = response.Model,
                CreatedAtUnixTime = response.CreatedUnixTime,
                Events = response.Events.ToList(),
                FineTunedModel = response.FineTunedModel,
                HyperParams = response.HyperParams,
                OrganizationId = response.OrganizationId,
                ResultFiles = response.ResultFiles.ToList(),
                Status = response.Status,
                ValidationFiles = response.ValidationFiles.ToList(),
                TrainingFiles = response.TrainingFiles.ToList(),
                UpdatedAtUnixTime = response.UpdatedAtUnixTime
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