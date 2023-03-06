using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatRequest
    {
        public ChatRequest(
            IEnumerable<ChatPrompt> messages,
            Model model = null,
            double? temperature = null,
            double? topP = null,
            int? number = null,
            string[] stops = null,
            int? maxTokens = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            Dictionary<string, double> logitBias = null,
            string user = null)
        {
            const string defaultModel = "gpt-3.5-turbo";
            Model = model ?? Models.Model.GPT3_5_Turbo;

            if (!Model.Contains(defaultModel))
            {
                throw new ArgumentException(nameof(model), $"{Model} not supported");
            }

            Messages = messages.ToList();
            Temperature = temperature;
            TopP = topP;
            Number = number;
            Stops = stops;
            MaxTokens = maxTokens;
            PresencePenalty = presencePenalty;
            FrequencyPenalty = frequencyPenalty;
            LogitBias = logitBias;
            User = user;
        }

        /// <summary>
        /// ID of the model to use. Currently, only gpt-3.5-turbo and gpt-3.5-turbo-0301 are supported.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// The messages to generate chat completions for, in the chat format.
        /// </summary>
        [JsonPropertyName("messages")]
        public IReadOnlyList<ChatPrompt> Messages { get; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2.
        /// Higher values like 0.8 will make the output more random, while lower values like 0.2 will
        /// make it more focused and deterministic.
        /// We generally recommend altering this or top_p but not both.<br/>
        /// Defaults to 1
        /// </summary>
        [JsonPropertyName("temperature")]
        public double? Temperature { get; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.<br/>
        /// Defaults to 1
        /// </summary>
        [JsonPropertyName("top_p")]
        public double? TopP { get; }

        /// <summary>
        /// How many chat completion choices to generate for each input message.<br/>
        /// Defaults to 1
        /// </summary>
        [JsonPropertyName("n")]
        public int? Number { get; }

        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// Do not set this yourself, use the appropriate methods on <see cref="ChatEndpoint"/> instead.<br/>
        /// Defaults to false
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; internal set; }

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// </summary>
        [JsonPropertyName("stop")]
        public string[] Stops { get; }

        /// <summary>
        /// The maximum number of tokens allowed for the generated answer.
        /// By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; }

        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.<br/>
        /// Defaults to 0
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; }

        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on their existing frequency in the text so far,
        /// decreasing the model's likelihood to repeat the same line verbatim.<br/>
        /// Defaults to 0
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; }

        /// <summary>Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens(specified by their token ID in the tokenizer)
        /// to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits
        /// generated by the model prior to sampling.The exact effect will vary per model, but values between
        /// -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result
        /// in a ban or exclusive selection of the relevant token.<br/>
        /// Defaults to null
        /// </summary>
        [JsonPropertyName("logit_bias")]
        public Dictionary<string, double> LogitBias { get; set; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}