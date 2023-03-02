// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatResponse
    {
        [JsonConstructor]
        public ChatResponse(
            string id,
            string @object,
            int created,
            string model,
            Usage usage, 
            List<Choice> choices
        )
        {
            Id = id;
            Object = @object;
            Created = created;
            Model = model;
            Usage = usage;
            Choices = choices;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("created")]
        public int Created { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; }

        [JsonPropertyName("choices")]
        public IReadOnlyList<Choice> Choices { get; }

        [JsonIgnore]
        public Choice FirstChoice => Choices.FirstOrDefault();

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
