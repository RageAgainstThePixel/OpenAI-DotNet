using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// Represents a response from the Classification API.
    /// <see href="https://beta.openai.com/docs/api-reference/classifications">the OpenAI docs</see>.
    /// </summary>
    public sealed class ClassificationResponse : BaseResponse
    {
        [JsonPropertyName("completion")]
        public string Completion { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("selected_examples")]
        public IReadOnlyList<SelectedExample> SelectedExamples { get; set; }
    }
}