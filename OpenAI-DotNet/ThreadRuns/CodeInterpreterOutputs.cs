using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.ThreadRuns
{
    public sealed class CodeInterpreterOutputs
    {
        /// <summary>
        /// Output type
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<CodeInterpreterOutputType>))]
        public CodeInterpreterOutputType Type { get; set; }
    
        /// <summary>
        /// The text output from the Code Interpreter tool call.
        /// </summary>
        [JsonPropertyName("logs")]
        public string Logs { get; set; }
    
        /// <summary>
        /// Code interpreter image output
        /// </summary>
        [JsonPropertyName("image")]
        public CodeInterpreterImageOutput Image { get; set; }
    }
}