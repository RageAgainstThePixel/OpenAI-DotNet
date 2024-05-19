// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Error
    {
        /// <summary>
        /// An error code identifying the error type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }

        /// <summary>
        /// A human-readable message providing more details about the error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message { get; private set; }

        /// <summary>
        /// The name of the parameter that caused the error, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("param")]
        public string Parameter { get; private set; }

        /// <summary>
        /// The type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The line number of the input file where the error occurred, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("line")]
        public int? Line { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"[{Code}]");

            if (!string.IsNullOrEmpty(Message))
            {
                builder.Append($" {Message}");
            }

            if (!string.IsNullOrEmpty(Type))
            {
                builder.Append($" Type: {Type}");
            }

            if (!string.IsNullOrEmpty(Parameter))
            {
                builder.Append($" Parameter: {Parameter}");
            }

            if (Line.HasValue)
            {
                builder.Append($" Line: {Line.Value}");
            }

            return builder.ToString();
        }
    }
}
