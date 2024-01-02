// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    public sealed class HyperParams
    {
        [JsonInclude]
        [JsonPropertyName("n_epochs")]
        public object Epochs { get; private set; }

        [JsonInclude]
        [JsonPropertyName("batch_size")]
        public object BatchSize { get; private set; }

        [JsonInclude]
        [JsonPropertyName("learning_rate_multiplier")]
        public object LearningRateMultiplier { get; private set; }
    }
}