using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class HyperParams
    {
        [JsonConstructor]
        public HyperParams(int? batchSize, double? learningRateMultiplier, int epochs, double promptLossWeight)
        {
            this.BatchSize = batchSize;
            this.LearningRateMultiplier = learningRateMultiplier;
            this.Epochs = epochs;
            this.PromptLossWeight = promptLossWeight;
        }

        [JsonInclude]
        [JsonPropertyName("batch_size")]
        public int? BatchSize { get; private set; }

        [JsonInclude]
        [JsonPropertyName("learning_rate_multiplier")]
        public double? LearningRateMultiplier { get; private set; }

        [JsonInclude]
        [JsonPropertyName("n_epochs")]
        public int Epochs { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prompt_loss_weight")]
        public double PromptLossWeight { get; private set; }
    }
}
