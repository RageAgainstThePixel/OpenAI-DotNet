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
            TrainingFileId = trainingFileId;
            ValidationFileId = validationFileId;
            Model = model ?? "curie";
            Epochs = (int)epochs;
            BatchSize = batchSize;
            LearningRateMultiplier = learningRateMultiplier;
            PromptLossWeight = promptLossWeight;
            ComputeClassificationMetrics = computeClassificationMetrics;
            ClassificationNClasses = classificationNClasses;
            ClassificationPositiveClasses = classificationPositiveClasses;
            ClassificationBetas = classificationBetas;
            Suffix = suffix;
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
