// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CodeInterpreterOutputs
    {
        /// <summary>
        /// Output type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<CodeInterpreterOutputType>))]
        public CodeInterpreterOutputType Type { get; private set; }

        /// <summary>
        /// The text output from the Code Interpreter tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logs")]
        public string Logs { get; private set; }

        /// <summary>
        /// Code interpreter image output
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("image")]
        public CodeInterpreterImageOutput Image { get; private set; }
    }
}