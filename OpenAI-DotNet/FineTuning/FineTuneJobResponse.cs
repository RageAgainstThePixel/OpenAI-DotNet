// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class FineTuneJobResponse : BaseResponse
    {
        public FineTuneJobResponse() { }

#pragma warning disable CS0618 // Type or member is obsolete
        internal FineTuneJobResponse(FineTuneJob job)
        {
            Object = job.Object;
            Id = job.Id;
            Model = job.Model;
            CreateAtUnixTimeSeconds = job.CreateAtUnixTimeSeconds;
            FinishedAtUnixTimeSeconds = job.FinishedAtUnixTimeSeconds;
            FineTunedModel = job.FineTunedModel;
            OrganizationId = job.OrganizationId;
            ResultFiles = job.ResultFiles;
            Status = job.Status;
            ValidationFile = job.ValidationFile;
            TrainingFile = job.TrainingFile;
            HyperParameters = job.HyperParameters;
            TrainedTokens = job.TrainedTokens;
            events = new List<EventResponse>(job.Events.Count);

            foreach (var jobEvent in job.Events)
            {
                jobEvent.Client = Client;
                events.Add(jobEvent);
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

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
        public DateTime? CreatedAt
            => CreateAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CreateAtUnixTimeSeconds.Value).DateTime
                : null;

        [JsonInclude]
        [JsonPropertyName("finished_at")]
        public int? FinishedAtUnixTimeSeconds { get; private set; }

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

        private List<EventResponse> events = new();

        [JsonIgnore]
        public IReadOnlyList<EventResponse> Events
        {
            get => events;
            internal set
            {
                events = value?.ToList() ?? new List<EventResponse>();

                foreach (var @event in events)
                {
                    @event.Client = Client;
                }
            }
        }

        public static implicit operator string(FineTuneJobResponse job) => job?.ToString();

        public override string ToString() => Id;
    }
}