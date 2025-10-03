// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class ReasoningSummary : IServerSentEvent
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// A short summary of the reasoning used by the model when generating the response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; internal set; }

        private string delta;

        [JsonIgnore]
        public string Delta
        {
            get => delta;
            internal set
            {
                if (value == null)
                {
                    delta = null;
                }
                else
                {
                    delta += value;
                }
            }
        }

        [JsonIgnore]
        public string Object
            => Type;

        public string ToJsonString()
            => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);

        public override string ToString()
            => Delta ?? Text ?? string.Empty;
    }
}
