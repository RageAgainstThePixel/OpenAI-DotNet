using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatResponse : BaseResponse
    {
        public ChatResponse() { }

        internal ChatResponse(ChatResponse other) => CopyFrom(other);

        public ChatResponse(
            string id,
            string @object,
            int created,
            string model,
            Usage usage,
            List<Choice> choices)
            : this()
        {
            Id = id;
            Object = @object;
            Created = created;
            Model = model;
            Usage = usage;
            Choices = choices;
        }

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created")]
        public int Created { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("usage")]
        public Usage Usage { get; private set; }

        [JsonIgnore]
        private List<Choice> choices;

        [JsonInclude]
        [JsonPropertyName("choices")]
        public IReadOnlyList<Choice> Choices
        {
            get => choices;
            private set => choices = value.ToList();
        }

        [JsonIgnore]
        public Choice FirstChoice => Choices?.FirstOrDefault(choice => choice.Index == 0);

        public override string ToString() => FirstChoice?.ToString() ?? string.Empty;

        public static implicit operator string(ChatResponse response) => response.ToString();

        internal void CopyFrom(ChatResponse other)
        {
            if (!string.IsNullOrWhiteSpace(other?.Id))
            {
                Id = other.Id;
            }

            if (!string.IsNullOrEmpty(other?.Object))
            {
                Object = other.Object;
            }

            if (!string.IsNullOrWhiteSpace(other?.Model))
            {
                Model = other.Model;
            }

            if (other?.Usage != null)
            {
                if (Usage == null)
                {
                    Usage = other.Usage;
                }
                else
                {
                    Usage.CopyFrom(other.Usage);
                }
            }

            if (other?.Choices is { Count: > 0 })
            {
                choices ??= new List<Choice>();

                foreach (var otherChoice in other.Choices)
                {
                    if (otherChoice.Index + 1 > choices.Count)
                    {
                        choices.Insert(otherChoice.Index, otherChoice);
                    }

                    choices[otherChoice.Index].CopyFrom(otherChoice);
                }
            }
        }

        public string GetUsage(bool log = true)
        {
            var message = $"{Id} | {Model} | {Usage}";

            if (log)
            {
                Console.WriteLine(message);
            }

            return message;
        }
    }
}
