// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatResponse : BaseResponse, IServerSentEvent
    {
        public ChatResponse() { }

        internal ChatResponse(ChatResponse other) => AppendFrom(other);

        /// <summary>
        /// A unique identifier for the chat completion.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) of when the chat completion was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("service_tier")]
        public string ServiceTier { get; private set; }

        /// <summary>
        /// This fingerprint represents the backend configuration that the model runs with.
        /// Can be used in conjunction with the seed request parameter to understand when
        /// backend changes have been made that might impact determinism.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; private set; }

        [JsonInclude]
        [JsonPropertyName("usage")]
        public Usage Usage { get; private set; }

        [JsonIgnore]
        private List<Choice> choices;

        /// <summary>
        /// A list of chat completion choices. Can be more than one if n is greater than 1.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("choices")]
        public IReadOnlyList<Choice> Choices
        {
            get => choices;
            private set => choices = value?.ToList();
        }

        [JsonIgnore]
        public Choice FirstChoice
            => Choices?.FirstOrDefault(choice => choice.Index == 0);

        public override string ToString()
            => FirstChoice?.ToString() ?? string.Empty;

        public static implicit operator string(ChatResponse response) => response?.ToString();

        internal void AppendFrom(ChatResponse other)
        {
            if (other is null) { return; }

            if (!string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id))
            {
                if (Id != other.Id)
                {
                    throw new InvalidOperationException($"Attempting to append a different object than the original! {Id} != {other.Id}");
                }
            }
            else
            {
                Id = other.Id;
            }

            if (!string.IsNullOrWhiteSpace(other.Object))
            {
                Object = other.Object;
            }

            if (!string.IsNullOrWhiteSpace(other.Model))
            {
                Model = other.Model;
            }

            if (other.Usage != null)
            {
                if (Usage == null)
                {
                    Usage = other.Usage;
                }
                else
                {
                    Usage.AppendFrom(other.Usage);
                }
            }

            if (other.Choices is { Count: > 0 })
            {
                choices ??= new List<Choice>();
                choices.AppendFrom(other.Choices);
            }
        }

        public string GetUsage(bool log = true)
        {
            if (Usage == null) { return string.Empty; }

            var message = $"{Id} | {Model} | {Usage}";

            if (log)
            {
                Console.WriteLine(message);
            }

            return message;
        }
    }
}
