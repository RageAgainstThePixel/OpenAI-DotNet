using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI_DotNet
{
    /// <summary>
    /// Represents a result from calling the Completion API
    /// </summary>
    public class CompletionResult
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
        /// Which model was used to generate this result.  Be sure to check <see cref="OpenAI_DotNet.Engine.ModelRevision"/> for the specific revision.
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
        /// The server-side processing time as reported by the API.  This can be useful for debugging where a delay occurs.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ProcessingTime { get; set; }

        /// <summary>
        /// The organization associated with the API request, as reported by the API.
        /// </summary>
        [JsonIgnore]
        public string Organization { get; set; }

        /// <summary>
        /// The request id of this API call, as reported in the response headers.  This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
        /// </summary>
        [JsonIgnore]
        public string RequestId { get; set; }

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
