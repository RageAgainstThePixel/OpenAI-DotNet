using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.FineTuning
{
    /// <summary>
    /// To fine-tune a model, you'll need a set of training examples that each consist of a single input
    /// ("prompt") and its associated output ("completion"). This is notably different from using our base models,
    /// where you might input detailed instructions or multiple examples in a single prompt.
    /// <see href="https://beta.openai.com/docs/guides/fine-tuning/specific-guidelines"/>
    /// </summary>
    public class FineTuningTrainingData
    {
        public static implicit operator string(FineTuningTrainingData data) => JsonSerializer.Serialize(data);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prompt">Prompt text.</param>
        /// <param name="completion">The ideal completion text.</param>
        public FineTuningTrainingData(string prompt, string completion)
        {
            Prompt = $"{prompt}";
            Completion = $" {completion}";
        }

        /// <summary>
        /// Prompt text.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; }

        /// <summary>
        /// The ideal completion text.
        /// </summary>
        [JsonPropertyName("completion")]
        public string Completion { get; }
    }
}
