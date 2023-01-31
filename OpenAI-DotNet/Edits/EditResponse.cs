using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Edits
{
    public sealed class EditResponse : BaseResponse
    {
        [JsonConstructor]
        public EditResponse(string @object, int createdUnixTime, List<Choice> choices, Usage usage)
        {
            Object = @object;
            CreatedUnixTime = createdUnixTime;
            Choices = choices;
            Usage = usage;
        }

        [JsonPropertyName("object")]
        public string Object { get; }

        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public int CreatedUnixTime { get; }

        /// The time when the result was generated
        [JsonIgnore]
        public DateTime Created => DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime).DateTime;

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; }

        /// <summary>
        /// Gets the text of the first edit, representing the main result
        /// </summary>
        public override string ToString()
        {
            return Choices is { Count: > 0 }
                ? Choices[0]
                : "Edit result has no valid output";
        }
    }
}
