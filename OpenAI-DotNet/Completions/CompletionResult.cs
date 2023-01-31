using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Represents a result from calling the <see cref="CompletionsEndpoint"/>.
    /// </summary>
    public sealed class CompletionResult : BaseResponse
    {
        [JsonConstructor]
        public CompletionResult(string id, string @object, int createdUnixTime, string model, List<Choice> completions)
        {
            Id = id;
            Object = @object;
            CreatedUnixTime = createdUnixTime;
            Model = model;
            Completions = completions;
        }

        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public int CreatedUnixTime { get; }

        /// <summary>
        /// The time when the result was generated.
        /// </summary>
        [JsonIgnore]
        public DateTime Created => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// The completions returned by the API.  Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<Choice> Completions { get; }

        /// <summary>
        /// Gets the text of the first completion, representing the main result
        /// </summary>
        public override string ToString()
        {
            return Completions is { Count: > 0 }
                ? Completions[0]
                : $"CompletionResult {Id} has no valid output";
        }
    }
}
