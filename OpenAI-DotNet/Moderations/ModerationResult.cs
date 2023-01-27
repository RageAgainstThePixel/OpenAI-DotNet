using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationResult
    {
        [JsonConstructor]
        public ModerationResult(Categories categories, Scores scores, bool flagged)
        {
            Categories = categories;
            Scores = scores;
            Flagged = flagged;
        }

        [JsonPropertyName("categories")]
        public Categories Categories { get; }

        [JsonPropertyName("category_scores")]
        public Scores Scores { get; }

        [JsonPropertyName("flagged")]
        public bool Flagged { get; }
    }
}