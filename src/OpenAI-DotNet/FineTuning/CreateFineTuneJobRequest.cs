using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class CreateFineTuneJobRequest
    {
        public CreateFineTuneJobRequest(
            string trainingFileId,
            string validationFileId = null,
            string model = null,
            uint epochs = 4,
            double? batchSize = null,
            double? learningRateMultiplier = null,
            double promptLossWeight = 0.01d,
            bool computeClassificationMetrics = false,
            int? classificationNClasses = null,
            string classificationPositiveClasses = null,
            IReadOnlyList<double> classificationBetas = null,
            string suffix = null)
        {
            this.TrainingFileId = trainingFileId;
            this.ValidationFileId = validationFileId;
            this.Model = model ?? "curie";
            this.Epochs = (int)epochs;
            this.BatchSize = batchSize;
            this.LearningRateMultiplier = learningRateMultiplier;
            this.PromptLossWeight = promptLossWeight;
            this.ComputeClassificationMetrics = computeClassificationMetrics;
            this.ClassificationNClasses = classificationNClasses;
            this.ClassificationPositiveClasses = classificationPositiveClasses;
            this.ClassificationBetas = classificationBetas;
            this.Suffix = suffix;
        }

        [JsonPropertyName("training_file")]
        public string TrainingFileId { get; set; }

        [JsonPropertyName("validation_file")]
        public string ValidationFileId { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("n_epochs")]
        public int Epochs { get; set; }

        [JsonPropertyName("batch_size")]
        public double? BatchSize { get; set; }

        [JsonPropertyName("learning_rate_multiplier")]
        public double? LearningRateMultiplier { get; set; }

        [JsonPropertyName("prompt_loss_weight")]
        public double PromptLossWeight { get; set; }

        [JsonPropertyName("compute_classification_metrics")]
        public bool ComputeClassificationMetrics { get; set; }

        [JsonPropertyName("classification_n_classes")]
        public int? ClassificationNClasses { get; set; }

        [JsonPropertyName("classification_positive_class")]
        public string ClassificationPositiveClasses { get; set; }

        [JsonPropertyName("classification_betas")]
        public IReadOnlyList<double> ClassificationBetas { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }
    }
}
