// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// An invocation of an Assistant on a Thread.
    /// The Assistant uses it's configuration and the Thread's Messages to perform tasks by calling models and tools.
    /// As part of a Run, the Assistant appends Messages to the Thread.
    /// </summary>
    public sealed class RunResponse : BaseResponse, IServerSentEvent
    {
        public RunResponse() { }

        internal RunResponse(RunResponse other) => AppendFrom(other);

        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the thread was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The thread ID that this run belongs to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The ID of the assistant used for execution of this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }

        /// <summary>
        /// The status of the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<RunStatus>))]
        public RunStatus Status { get; private set; }

        /// <summary>
        /// Details on the action required to continue the run.
        /// Will be null if no action is required.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("required_action")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public RequiredAction RequiredAction { get; private set; }

        /// <summary>
        /// The Last error Associated with this run.
        /// Will be null if there are no errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Error LastError { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run will expire.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expires_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ExpiresAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? ExpiresAt
            => ExpiresAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was started.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("started_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? StartedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? StartedAt
            => StartedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(StartedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was cancelled.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cancelled_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CancelledAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CancelledAt
            => CancelledAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CancelledAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? FailedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? FailedAt
            => FailedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(FailedAtUnixTimeSeconds.Value).DateTime
                : null;

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was completed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CompletedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? CompletedAt
            => CompletedAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(CompletedAtUnixTimeSeconds.Value).DateTime
                : null;

        [JsonInclude]
        [JsonPropertyName("incomplete_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IncompleteDetails IncompleteDetails { get; private set; }

        /// <summary>
        /// The model that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("model")]
        public string Model { get; private set; }

        /// <summary>
        /// The instructions that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("instructions")]
        public string Instructions { get; private set; }

        private List<Tool> tools;

        /// <summary>
        /// The list of tools that the assistant used for this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<Tool> Tools
        {
            get => tools;
            private set => tools = value?.ToList();
        }

        /// <summary>
        /// The list of File IDs the assistant used for this run.
        /// </summary>
        [JsonIgnore]
        [Obsolete("Removed")]
        public IReadOnlyList<string> FileIds => null;

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// Usage statistics related to the run. This value will be `null` if the run is not in a terminal state (i.e. `in_progress`, `queued`, etc.).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("usage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Usage Usage { get; private set; }

        /// <summary>
        /// The sampling temperature used for this run. If not set, defaults to 1.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double? Temperature { get; private set; }

        /// <summary>
        /// The nucleus sampling value used for this run. If not set, defaults to 1.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("top_p")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double? TopP { get; private set; }

        /// <summary>
        /// The maximum number of prompt tokens specified to have been used over the course of the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("max_prompt_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? MaxPromptTokens { get; private set; }

        /// <summary>
        /// The maximum number of completion tokens specified to have been used over the course of the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("max_completion_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? MaxCompletionTokens { get; private set; }

        /// <summary>
        /// Controls for how a thread will be truncated prior to the run. Use this to control the initial context window of the run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("truncation_strategy")]
        public TruncationStrategy TruncationStrategy { get; private set; }

        /// <summary>
        /// Controls which (if any) tool is called by the model.
        /// none means the model will not call any tools and instead generates a message.
        /// auto is the default value and means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools before responding to the user.
        /// Specifying a particular tool like {"type": "file_search"} or {"type": "function", "function": {"name": "my_function"}}
        /// forces the model to call that tool.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_choice")]
        public dynamic ToolChoice { get; private set; }

        [JsonInclude]
        [JsonPropertyName("parallel_tool_calls")]
        public bool ParallelToolCalls { get; private set; }

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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResponseFormatObject ResponseFormatObject { get; private set; }

        [JsonIgnore]
        public ChatResponseFormat ResponseFormat => ResponseFormatObject ?? ChatResponseFormat.Auto;

        public static implicit operator string(RunResponse run) => run?.ToString();

        public override string ToString() => Id;

        internal void AppendFrom(RunResponse other)
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

            Object = other.Object;

            if (other.Status > 0)
            {
                Status = other.Status;
            }

            if (other.RequiredAction != null)
            {
                RequiredAction = other.RequiredAction;
            }

            if (other.LastError != null)
            {
                LastError = other.LastError;
            }

            if (other.ExpiresAtUnixTimeSeconds.HasValue)
            {
                ExpiresAtUnixTimeSeconds = other.ExpiresAtUnixTimeSeconds;
            }

            if (other.StartedAtUnixTimeSeconds.HasValue)
            {
                StartedAtUnixTimeSeconds = other.StartedAtUnixTimeSeconds;
            }

            if (other.CancelledAtUnixTimeSeconds.HasValue)
            {
                CancelledAtUnixTimeSeconds = other.CancelledAtUnixTimeSeconds;
            }

            if (other.FailedAtUnixTimeSeconds.HasValue)
            {
                FailedAtUnixTimeSeconds = other.FailedAtUnixTimeSeconds;
            }

            if (other.CompletedAtUnixTimeSeconds.HasValue)
            {
                CompletedAtUnixTimeSeconds = other.CompletedAtUnixTimeSeconds;
            }

            if (other.IncompleteDetails != null)
            {
                IncompleteDetails = other.IncompleteDetails;
            }

            if (other is { Tools: not null })
            {
                tools ??= new List<Tool>();
                tools.AppendFrom(other.Tools);
            }

            if (other.Metadata is { Count: > 0 })
            {
                Metadata = other.Metadata;
            }

            if (other.Usage != null)
            {
                Usage = other.Usage;
            }

            if (other.Temperature.HasValue)
            {
                Temperature = other.Temperature;
            }

            if (other.TopP.HasValue)
            {
                TopP = other.TopP;
            }

            if (other.MaxPromptTokens.HasValue)
            {
                MaxPromptTokens = other.MaxPromptTokens;
            }

            if (other.MaxCompletionTokens.HasValue)
            {
                MaxCompletionTokens = other.MaxCompletionTokens;
            }

            if (other.TruncationStrategy != null)
            {
                TruncationStrategy = other.TruncationStrategy;
            }

            if (other.ToolChoice is string stringToolChoice)
            {
                ToolChoice = stringToolChoice;
            }
            else
            {
                ToolChoice = other.ToolChoice;
            }

            if (other.ResponseFormatObject != null)
            {
                ResponseFormatObject = other.ResponseFormatObject;
            }
        }
    }
}
