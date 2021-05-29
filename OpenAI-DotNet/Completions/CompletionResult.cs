using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI_DotNet
{
    /// <summary>
    /// Represents a result from calling the <see cref="CompletionEndpoint"/>.
    /// </summary>
    public sealed class CompletionResult : BaseResponse
    {
        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public int CreatedUnixTime { get; set; }

        /// The time when the result was generated
        [JsonIgnore]
        public DateTime Created => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        private Engine engine;

        /// <summary>
        /// Which model was used to generate this result. Be sure to check <see cref="OpenAI_DotNet.Engine.ModelRevision"/> for the specific revision.
        /// </summary>
        [JsonIgnore]
        public Engine Engine
        {
            get => engine ?? new Engine(Model);
            set => engine = value;
        }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The completions returned by the API.  Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<Choice> Completions { get; set; }

        /// <summary>
        /// Gets the text of the first completion, representing the main result
        /// </summary>
        public override string ToString()
        {
            return Completions != null && Completions.Count > 0
                ? Completions[0].ToString()
                : $"CompletionResult {Id} has no valid output";
        }
    }
}
