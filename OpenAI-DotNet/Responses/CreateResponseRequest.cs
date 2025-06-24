﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class CreateResponseRequest
    {
        public static implicit operator CreateResponseRequest(string textInput) =>
            new(new List<IResponseItem> { new Message(Role.User, new TextContent(textInput)) });

        public CreateResponseRequest() { }

        public CreateResponseRequest(
            string textInput,
            Model model = null,
            bool? background = null,
            IEnumerable<string> include = null,
            string instructions = null,
            int? maxOutputTokens = null,
            IReadOnlyDictionary<string, string> metadata = null,
            bool? parallelToolCalls = null,
            string previousResponseId = null,
            Prompt prompt = null,
            Reasoning reasoning = null,
            string serviceTier = null,
            bool? store = null,
            double? temperature = null,
            TextResponseFormat responseFormat = TextResponseFormat.Auto,
            JsonSchema jsonSchema = null,
            string toolChoice = null,
            IEnumerable<Tool> tools = null,
            double? topP = null,
            Truncation truncation = Truncation.Auto,
            string user = null)
            : this(
                input: new List<IResponseItem> { new Message(Role.User, new TextContent(textInput)) },
                model: model,
                background: background,
                include: include,
                instructions: instructions,
                maxOutputTokens: maxOutputTokens,
                metadata: metadata,
                parallelToolCalls: parallelToolCalls,
                previousResponseId: previousResponseId,
                prompt: prompt,
                reasoning: reasoning,
                serviceTier: serviceTier,
                store: store,
                temperature: temperature,
                responseFormat: responseFormat,
                jsonSchema: jsonSchema,
                toolChoice: toolChoice,
                tools: tools,
                topP: topP,
                truncation: truncation,
                user: user)
        {
        }

        public CreateResponseRequest(
            IEnumerable<IResponseItem> input,
            Model model = null,
            bool? background = null,
            IEnumerable<string> include = null,
            string instructions = null,
            int? maxOutputTokens = null,
            IReadOnlyDictionary<string, string> metadata = null,
            bool? parallelToolCalls = null,
            string previousResponseId = null,
            Prompt prompt = null,
            Reasoning reasoning = null,
            string serviceTier = null,
            bool? store = null,
            double? temperature = null,
            TextResponseFormat responseFormat = TextResponseFormat.Auto,
            JsonSchema jsonSchema = null,
            string toolChoice = null,
            IEnumerable<Tool> tools = null,
            double? topP = null,
            Truncation truncation = Truncation.Auto,
            string user = null)
        {
            Input = input?.ToArray() ?? throw new ArgumentNullException(nameof(input));
            Model = string.IsNullOrWhiteSpace(model) ? Models.Model.ChatGPT4o : model;
            Background = background;
            Include = include?.ToList();
            Instructions = instructions;
            MaxOutputTokens = maxOutputTokens;
            Metadata = metadata;
            ParallelToolCalls = parallelToolCalls;
            PreviousResponseId = previousResponseId;
            Prompt = prompt;
            Reasoning = reasoning;
            ServiceTier = serviceTier;
            Store = store;
            Temperature = temperature;
            TextResponseFormatObject = responseFormat;

            if (jsonSchema != null)
            {
                TextResponseFormatObject = jsonSchema;
            }
            else
            {
                TextResponseFormatObject = responseFormat switch
                {
                    TextResponseFormat.Auto => null,
                    _ => responseFormat
                };
            }

            tools.ProcessTools<ITool>(toolChoice, out var toolList, out var activeTool);
            Tools = toolList;
            ToolChoice = activeTool;
            TopP = topP;
            Truncation = truncation;
            User = user;
        }

        /// <summary>
        /// Text, image, or file inputs to the model, used to generate a response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input")]
        public IReadOnlyList<IResponseItem> Input { get; private set; }

        /// <summary>
        /// Model ID used to generate the response, like gpt-4o or o3.
        /// OpenAI offers a wide range of models with different capabilities, performance characteristics, and price points.
        /// Refer to the model guide to browse and compare available models.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// Whether to run the model response in the background. 
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("background")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Background { get; private set; }

        /// <summary>
        /// Specify additional output data to include in the model response.<br/>
        /// Currently supported values are:<br/>
        /// - file_search_call.results: Include the search results of the file search tool call.<br/>
        /// - message.input_image.image_url: Include image URLs from the computer call output.<br/>
        /// - computer_call_output.output.image_url: Include image urls from the computer call output.<br/>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("include")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> Include { get; private set; }

        /// <summary>
        /// Inserts a system (or developer) message as the first item in the model's context.
        /// When using along with <see cref="PreviousResponseId"/>,
        /// the instructions from a previous response will not be carried over to the next response.
        /// This makes it simple to swap out system (or developer) messages in new responses.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("instructions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Instructions { get; private set; }

        /// <summary>
        /// An upper bound for the number of tokens that can be generated for a
        /// response, including visible output tokens and reasoning
        /// tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("max_output_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxOutputTokens { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format,
        /// and querying for objects via API or the dashboard.
        /// Keys are strings with a maximum length of 64 characters.
        /// Values are strings with a maximum length of 512 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// Whether to allow the model to run tool calls in parallel.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parallel_tool_calls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ParallelToolCalls { get; private set; }

        /// <summary>
        /// The unique ID of the previous response to the model.
        /// Use this to create multi-turn conversations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("previous_response_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PreviousResponseId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prompt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Prompt Prompt { get; private set; }

        /// <summary>
        /// Configuration options for reasoning models.
        /// </summary>
        /// <remarks>
        /// o-series models only!
        /// </remarks>
        [JsonInclude]
        [JsonPropertyName("reasoning")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Reasoning Reasoning { get; private set; }

        /// <summary>
        /// Specifies the latency tier to use for processing the request. This parameter is relevant for customers subscribed to the scale tier service:<br/>
        /// - If set to 'auto', and the Project is Scale tier enabled, the system will utilize scale tier credits until they are exhausted.<br/>
        /// - If set to 'auto', and the Project is not Scale tier enabled, the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.<br/>
        /// - If set to 'default', the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.<br/>
        /// - When not set, the default behavior is 'auto'.<br/>
        /// When this parameter is set, the response body will include the service_tier utilized.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("service_tier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ServiceTier { get; private set; }

        /// <summary>
        /// Whether to store the generated model response for later retrieval via API.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("store")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Store { get; private set; }

        /// <summary>
        /// If set to true, the model response data will be streamed to the client as it is generated using server-sent events.
        /// </summary>
        [JsonInclude]
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
        [JsonInclude]
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Temperature { get; private set; }

        /// <summary>
        /// Configuration options for a text response from the model. Can be plain text or structured JSON data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextResponseFormatObject TextResponseFormatObject { get; internal set; }

        [JsonIgnore]
        public TextResponseFormat TextResponseFormat => TextResponseFormatObject ?? TextResponseFormat.Auto;

        /// <summary>
        /// How the model should select which tool (or tools) to use when generating a response.
        /// See the tools parameter to see how to specify which tools the model can call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_choice")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object ToolChoice { get; private set; }

        /// <summary>
        /// A list of tools the model may call. Currently, only functions are supported as a tool.
        /// Use this to provide a list of functions the model may generate JSON inputs for.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<ITool> Tools { get; private set; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.<br/>
        /// Defaults to 1
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("top_p")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TopP { get; private set; }

        /// <summary>
        /// The truncation strategy to use for the model response.<br/>
        /// - Auto: If the context of this response and previous ones exceeds the model's context window size,
        /// the model will truncate the response to fit the context window by dropping input items in the middle of the conversation.<br/>
        /// - Disabled (default): If a model response will exceed the context window size for a model, the request will fail with a 400 error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("truncation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Truncation Truncation { get; private set; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string User { get; private set; }
    }
}
