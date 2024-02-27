// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatRequest
    {
        /// <inheritdoc />
        public ChatRequest(
            IEnumerable<Message> messages,
            IEnumerable<Tool> tools,
            string toolChoice = null,
            string model = null,
            double? frequencyPenalty = null,
            IReadOnlyDictionary<string, double> logitBias = null,
            int? maxTokens = null,
            int? number = null,
            double? presencePenalty = null,
            ChatResponseFormat responseFormat = ChatResponseFormat.Text,
            int? seed = null,
            string[] stops = null,
            double? temperature = null,
            double? topP = null,
            int? topLogProbs = null,
            string user = null)
            : this(messages, model, frequencyPenalty, logitBias, maxTokens, number, presencePenalty, responseFormat, seed, stops, temperature, topP, topLogProbs, user)
        {
            var toolList = tools?.ToList();

            if (toolList != null && toolList.Any())
            {
                if (string.IsNullOrWhiteSpace(toolChoice))
                {
                    ToolChoice = "auto";
                }
                else
                {
                    if (!toolChoice.Equals("none") &&
                        !toolChoice.Equals("auto"))
                    {
                        var tool = toolList.FirstOrDefault(t => t.Function.Name.Contains(toolChoice)) ??
                            throw new ArgumentException($"The specified tool choice '{toolChoice}' was not found in the list of tools");
                        ToolChoice = new { type = "function", function = new { name = tool.Function.Name } };
                    }
                    else
                    {
                        ToolChoice = toolChoice;
                    }
                }
            }

            Tools = toolList?.ToList();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messages">
        /// The list of messages for the current chat session.
        /// </param>
        /// <param name="model">
        /// Id of the model to use.
        /// </param>
        /// <param name="temperature">
        /// What sampling temperature to use, between 0 and 2.
        /// Higher values like 0.8 will make the output more random, while lower values like 0.2 will
        /// make it more focused and deterministic.
        /// We generally recommend altering this or top_p but not both.<br/>
        /// Defaults to 1
        /// </param>
        /// <param name="topP">
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.<br/>
        /// Defaults to 1
        /// </param>
        /// <param name="number">
        /// How many chat completion choices to generate for each input message.<br/>
        /// Defaults to 1
        /// </param>
        /// <param name="seed"></param>
        /// <param name="stops">
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// </param>
        /// <param name="maxTokens">
        /// The maximum number of tokens allowed for the generated answer.
        /// By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </param>
        /// <param name="presencePenalty">
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.<br/>
        /// Defaults to 0
        /// </param>
        /// <param name="responseFormat">
        /// An object specifying the format that the model must output.
        /// Setting to <see cref="ChatResponseFormat.Json"/> enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.
        /// </param>
        /// <param name="frequencyPenalty">
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on their existing frequency in the text so far,
        /// decreasing the model's likelihood to repeat the same line verbatim.<br/>
        /// Defaults to 0
        /// </param>
        /// <param name="logitBias">
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens(specified by their token ID in the tokenizer)
        /// to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits
        /// generated by the model prior to sampling.The exact effect will vary per model, but values between
        /// -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result
        /// in a ban or exclusive selection of the relevant token.<br/>
        /// Defaults to null
        /// </param>
        /// <param name="topLogProbs">
        /// An integer between 0 and 5 specifying the number of most likely tokens to return at each token position,
        /// each with an associated log probability.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        public ChatRequest(
            IEnumerable<Message> messages,
            string model = null,
            double? frequencyPenalty = null,
            IReadOnlyDictionary<string, double> logitBias = null,
            int? maxTokens = null,
            int? number = null,
            double? presencePenalty = null,
            ChatResponseFormat responseFormat = ChatResponseFormat.Text,
            int? seed = null,
            string[] stops = null,
            double? temperature = null,
            double? topP = null,
            int? topLogProbs = null,
            string user = null)
        {
            Messages = messages?.ToList();

            if (Messages?.Count == 0)
            {
                throw new ArgumentNullException(nameof(messages), $"Missing required {nameof(messages)} parameter");
            }

            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.GPT3_5_Turbo : model;
            FrequencyPenalty = frequencyPenalty;
            LogitBias = logitBias;
            MaxTokens = maxTokens;
            Number = number;
            PresencePenalty = presencePenalty;
            ResponseFormat = ChatResponseFormat.Json == responseFormat ? responseFormat : null;
            Seed = seed;
            Stops = stops;
            Temperature = temperature;
            TopP = topP;
            LogProbs = topLogProbs.HasValue ? topLogProbs.Value > 0 : null;
            TopLogProbs = topLogProbs;
            User = user;
        }

        /// <summary>
        /// The messages to generate chat completions for, in the chat format.
        /// </summary>
        [JsonPropertyName("messages")]
        public IReadOnlyList<Message> Messages { get; }

        /// <summary>
        /// ID of the model to use.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; }

        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on their existing frequency in the text so far,
        /// decreasing the model's likelihood to repeat the same line verbatim.<br/>
        /// Defaults to 0
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; }

        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens(specified by their token ID in the tokenizer)
        /// to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits
        /// generated by the model prior to sampling.The exact effect will vary per model, but values between
        /// -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result
        /// in a ban or exclusive selection of the relevant token.<br/>
        /// Defaults to null
        /// </summary>
        [JsonPropertyName("logit_bias")]
        public IReadOnlyDictionary<string, double> LogitBias { get; }

        /// <summary>
        /// Whether to return log probabilities of the output tokens or not.
        /// If true, returns the log probabilities of each output token returned in the content of message.
        /// </summary>
        /// <remarks>
        /// This option is currently not available on the gpt-4-vision-preview model.
        /// </remarks>
        [JsonPropertyName("logprobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? LogProbs { get; }

        /// <summary>
        /// An integer between 0 and 5 specifying the number of most likely tokens to return at each token position,
        /// each with an associated log probability.
        /// </summary>
        /// <remarks>
        /// <see cref="LogProbs"/> must be set to true if this parameter is used.
        /// </remarks>
        [JsonPropertyName("top_logprobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? TopLogProbs { get; }

        /// <summary>
        /// The maximum number of tokens allowed for the generated answer.
        /// By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; }

        /// <summary>
        /// How many chat completion choices to generate for each input message.<br/>
        /// Defaults to 1
        /// </summary>
        [JsonPropertyName("n")]
        public int? Number { get; }

        /// <summary>
        /// Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.<br/>
        /// Defaults to 0
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; }

        /// <summary>
        /// An object specifying the format that the model must output.
        /// Setting to <see cref="ChatResponseFormat.Json"/> enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.
        /// </summary>
        /// <remarks>
        /// Important: When using JSON mode you must still instruct the model to produce JSON yourself via some conversation message,
        /// for example via your system message. If you don't do this, the model may generate an unending stream of
        /// whitespace until the generation reaches the token limit, which may take a lot of time and give the appearance
        /// of a "stuck" request. Also note that the message content may be partial (i.e. cut off) if finish_reason="length",
        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
        /// </remarks>
        [JsonPropertyName("response_format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResponseFormat ResponseFormat { get; }

        /// <summary>
        /// This feature is in Beta. If specified, our system will make a best effort to sample deterministically,
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter to
        /// monitor changes in the backend.
        /// </summary>
        [JsonPropertyName("seed")]
        public int? Seed { get; }

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// </summary>
        [JsonPropertyName("stop")]
        public string[] Stops { get; }

        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// Do not set this yourself, use the appropriate methods on <see cref="ChatEndpoint"/> instead.<br/>
        /// Defaults to false
        /// </summary>
        [JsonPropertyName("stream")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Stream { get; internal set; }

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
        /// A list of tools the model may call. Currently, only functions are supported as a tool.
        /// Use this to provide a list of functions the model may generate JSON inputs for.
        /// </summary>
        [JsonPropertyName("tools")]
        public IReadOnlyList<Tool> Tools { get; }

        /// <summary>
        /// Controls which (if any) function is called by the model.<br/>
        /// 'none' means the model will not call a function and instead generates a message.&lt;br/&gt;
        /// 'auto' means the model can pick between generating a message or calling a function.&lt;br/&gt;
        /// Specifying a particular function via {"type: "function", "function": {"name": "my_function"}}
        /// forces the model to call that function.<br/>
        /// 'none' is the default when no functions are present.<br/>
        /// 'auto' is the default if functions are present.<br/>
        /// </summary>
        [JsonPropertyName("tool_choice")]
        public dynamic ToolChoice { get; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; }

        /// <summary>
        /// Pass "auto" to let the OpenAI service decide, "none" if none are to be called,
        /// or "functionName" to force function call. Defaults to "auto".
        /// </summary>
        [Obsolete("Use ToolChoice")]
        [JsonPropertyName("function_call")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public dynamic FunctionCall { get; }

        /// <summary>
        /// An optional list of functions to get arguments for.
        /// </summary>
        [Obsolete("Use Tools")]
        [JsonPropertyName("functions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<Function> Functions { get; }

        /// <inheritdoc />
        public override string ToString() => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);
    }
}
