// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Models;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class CreateFineTuneJobRequest
    {
        public CreateFineTuneJobRequest(
            Model model,
            string trainingFileId,
            HyperParameters hyperParameters = null,
            string suffix = null,
            string validationFileId = null)
        {
            Model = model ?? Models.Model.GPT3_5_Turbo;
            TrainingFileId = trainingFileId;
            HyperParameters = hyperParameters;
            Suffix = suffix;
            ValidationFileId = validationFileId;
        }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("training_file")]
        public string TrainingFileId { get; set; }

        [JsonPropertyName("hyperparameters")]
        public HyperParameters HyperParameters { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        [JsonPropertyName("validation_file")]
        public string ValidationFileId { get; set; }
    }
}
