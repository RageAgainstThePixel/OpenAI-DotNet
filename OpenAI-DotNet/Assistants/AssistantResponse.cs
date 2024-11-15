// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Assistants
{
    /// <summary>
    /// Purpose-built AI that uses OpenAI's models and calls tools.
    /// </summary>
    public sealed class AssistantResponse : BaseResponse
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always assistant.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the assistant was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The name of the assistant.
        /// The maximum length is 256 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The description of the assistant.
        /// The maximum length is 512 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description { get; private set; }

        /// <summary>
        /// ID of the model to use.
        /// You can use the List models API to see all of your available models,
        /// or see our Model overview for descriptions of them.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// The system instructions that the assistant uses.
        /// The maximum length is 32768 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("instructions")]
        public string Instructions { get; private set; }

        /// <summary>
        /// A list of tool enabled on the assistant.
        /// There can be a maximum of 128 tools per assistant.
        /// Tools can be of types 'code_interpreter', 'retrieval', or 'function'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tools")]
        public IReadOnlyList<Tool> Tools { get; private set; }

        /// <summary>
        /// A set of resources that are used by the assistant's tools.
        /// The resources are specific to the type of tool.
        /// For example, the code_interpreter tool requires a list of file IDs,
        /// while the file_search tool requires a list of vector store IDs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_resources")]
        public ToolResources ToolResources { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("temperature")]
        public double Temperature { get; private set; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("top_p")]
        public double TopP { get; private set; }

        /// <summary>
        /// Specifies the format that the model must output.
        /// Setting to <see cref="ChatResponseFormat.Json"/> or <see cref="ChatResponseFormat.JsonSchema"/> enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.
        /// </summary>
        /// <remarks>
        /// Important: When using JSON mode you must still instruct the model to produce JSON yourself via some conversation message,
        /// for example via your system message. If you don't do this, the model may generate an unending stream of
        /// whitespace until the generation reaches the token limit, which may take a lot of time and give the appearance
        /// of a "stuck" request. Also note that the message content may be partial (i.e. cut off) if finish_reason="length",
        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
        /// </remarks>
        [JsonInclude]
        [JsonPropertyName("response_format")]
        [JsonConverter(typeof(ResponseFormatConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ResponseFormatObject ResponseFormatObject { get; private set; }

        [JsonIgnore]
        public ChatResponseFormat ResponseFormat => ResponseFormatObject ?? ChatResponseFormat.Auto;

        public static implicit operator string(AssistantResponse assistant) => assistant?.Id;

        public override string ToString() => Id;
    }
}
