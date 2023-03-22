using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    internal sealed class FineTuneJobResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedUnixTime { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonInclude]
        [JsonPropertyName("events")]
        public IReadOnlyList<Event> Events { get; private set; }

        [JsonInclude]
        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; private set; }

        [JsonInclude]
        [JsonPropertyName("hyperparams")]
        public HyperParams HyperParams { get; private set; }

        [JsonInclude]
        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("result_files")]
        public IReadOnlyList<FileData> ResultFiles { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("validation_files")]
        public IReadOnlyList<FileData> ValidationFiles { get; private set; }

        [JsonInclude]
        [JsonPropertyName("training_files")]
        public IReadOnlyList<FileData> TrainingFiles { get; private set; }

        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public int UpdatedAtUnixTime { get; private set; }

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
