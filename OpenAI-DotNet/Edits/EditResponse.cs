using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Edits
{
    [Obsolete("deprecated")]
    public sealed class EditResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [Obsolete("use CreatedAtUnixTimeSeconds")]
        public int CreatedUnixTime => CreatedAtUnixTimeSeconds;

        [JsonIgnore]
        [Obsolete("use CreatedAt")]
        public DateTime Created => CreatedAt;

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("choices")]
        public IReadOnlyList<Choice> Choices { get; private set; }

        [JsonInclude]
        [JsonPropertyName("usage")]
        public Usage Usage { get; private set; }

        /// <summary>
        /// Gets the text of the first edit, representing the main result
        /// </summary>
        public override string ToString()
            => Choices is { Count: > 0 }
                ? Choices[0]
                : "Edit result has no valid output";
    }
}
