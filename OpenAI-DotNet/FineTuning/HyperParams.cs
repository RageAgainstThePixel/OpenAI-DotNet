using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class HyperParams
    {
        [JsonConstructor]
        public HyperParams(int? batchSize, double? learningRateMultiplier, int epochs, double promptLossWeight)
        {
            BatchSize = batchSize;
            LearningRateMultiplier = learningRateMultiplier;
            Epochs = epochs;
            PromptLossWeight = promptLossWeight;
        }

        [JsonPropertyName("batch_size")]
        public int? BatchSize { get; }

        [JsonPropertyName("learning_rate_multiplier")]
        public double? LearningRateMultiplier { get; }

        [JsonPropertyName("n_epochs")]
        public int Epochs { get; }

        [JsonPropertyName("prompt_loss_weight")]
        public double PromptLossWeight { get; }
    }
}
