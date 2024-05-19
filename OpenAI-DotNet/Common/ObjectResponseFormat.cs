//// Licensed under the MIT License. See LICENSE in the project root for license information.

//using System.Text.Json.Serialization;
//using OpenAI.Extensions;

//namespace OpenAI
//{
//    public sealed class ObjectResponseFormat
//    {
//        public ObjectResponseFormat() => Type = ResponseFormat.Text;

//        public ObjectResponseFormat(ResponseFormat format) => Type = format;

//        /// <summary>
//        /// Specifies the format that the model must output.
//        /// Setting to <see cref="ResponseFormat.Json"/> enables JSON mode,
//        /// which guarantees the message the model generates is valid JSON.
//        /// </summary>
//        /// <remarks>
//        /// Important: When using JSON mode you must still instruct the model to produce JSON yourself via some conversation message,
//        /// for example via your system message. If you don't do this, the model may generate an unending stream of
//        /// whitespace until the generation reaches the token limit, which may take a lot of time and give the appearance
//        /// of a "stuck" request. Also note that the message content may be partial (i.e. cut off) if finish_reason="length",
//        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
//        /// </remarks>
//        [JsonInclude]
//        [JsonPropertyName("type")]
//        [JsonConverter(typeof(JsonStringEnumConverter<ResponseFormat>))]
//        public ResponseFormat Type { get; private set; }

//        public static implicit operator ResponseFormat(ObjectResponseFormat format) => format.Type;

//        public static implicit operator ObjectResponseFormat(ResponseFormat format) => new(format);
//    }
//}
