// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CodeInterpreterOutputs : IAppendable<CodeInterpreterOutputs>
    {
        [JsonInclude]
        [JsonPropertyName("index")]
        public int? Index { get; private set; }

        /// <summary>
        /// Output type. Can be either 'logs' or 'image'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<CodeInterpreterOutputType>))]
        public CodeInterpreterOutputType Type { get; private set; }

        /// <summary>
        /// Text output from the Code Interpreter tool call as part of a run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Logs { get; private set; }

        /// <summary>
        /// Code interpreter image output.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageFile Image { get; private set; }

        public void AppendFrom(CodeInterpreterOutputs other)
        {
            if (other == null) { return; }

            if (Type == 0 && other.Type > 0)
            {
                Type = other.Type;
            }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other.Logs))
            {
                Logs += other.Logs;
            }

            if (other.Image != null)
            {
                if (Image == null)
                {
                    Image = other.Image;
                }
                else
                {
                    Image.AppendFrom(other.Image);
                }
            }
        }
    }
}
