// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Error : BaseResponse, IServerSentEvent
    {
        public Error() { }

        internal Error(Exception e)
        {
            Type = e.GetType().Name;
            Message = e.Message;
            Exception = e;
        }

        /// <summary>
        /// An error code identifying the error type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Code { get; private set; }

        /// <summary>
        /// A human-readable message providing more details about the error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; private set; }

        /// <summary>
        /// The name of the parameter that caused the error, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("param")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Parameter { get; private set; }

        /// <summary>
        /// The type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Type { get; private set; }

        /// <summary>
        /// The line number of the input file where the error occurred, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("line")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Line { get; private set; }

        [JsonIgnore]
        public string Object => "error";

        [JsonIgnore]
        public Exception Exception { get; }

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

        public static implicit operator Exception(Error error)
            => error.Exception ?? new Exception(error.ToString());
    }
}
