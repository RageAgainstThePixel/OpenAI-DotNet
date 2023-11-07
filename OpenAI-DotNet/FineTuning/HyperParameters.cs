using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class HyperParameters
    {
        [JsonPropertyName("n_epochs")]
        public int? Epochs { get; set; }

        [JsonPropertyName("batch_size")]
        public int? BatchSize { get; set; }

        [JsonPropertyName("learning_rate_multiplier")]
        public int? LearningRateMultiplier { get; set; }
    }
}