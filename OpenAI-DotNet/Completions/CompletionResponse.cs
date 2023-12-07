using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Represents a result from calling the <see cref="CompletionsEndpoint"/>.
    /// </summary>
    [Obsolete("Deprecated")]
    public sealed class CompletionResponse : BaseResponse
    {
        public CompletionResponse() { }

#pragma warning disable CS0618 // Type or member is obsolete
        internal CompletionResponse(CompletionResult result)
        {
            Id = result.Id;
            Object = result.Object;
            CreatedUnixTimeSeconds = result.CreatedUnixTime;
            Model = result.Model;
            Completions = result.Completions;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created")]
        public int CreatedUnixTimeSeconds { get; private set; }

        /// <summary>
        /// The time when the result was generated.
        /// </summary>
        [JsonIgnore]
        public DateTime Created => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// The completions returned by the API.  Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("choices")]
        public IReadOnlyList<Choice> Completions { get; private set; }

        [JsonIgnore]
        public Choice FirstChoice => Completions?.FirstOrDefault(choice => choice.Index == 0);

        public override string ToString() => FirstChoice?.ToString() ?? string.Empty;

        public static implicit operator string(CompletionResponse response) => response?.ToString();
    }
}