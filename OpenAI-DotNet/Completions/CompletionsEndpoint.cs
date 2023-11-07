using OpenAI.Extensions;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Completions
{
    /// <summary>
    /// Text generation is the core function of the API. You give the API a prompt, and it generates a completion.
    /// The way you “program” the API to do a task is by simply describing the task in plain english or providing
    /// a few written examples. This simple approach works for a wide range of use cases, including summarization,
    /// translation, grammar correction, question answering, chatbots, composing emails, and much more
    /// (see the prompt library for inspiration).<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/completions"/>
    /// </summary>
    public sealed class CompletionsEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        internal CompletionsEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "completions";

        #region Non-streaming

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified parameters.
        /// This is non-streaming, so it will wait until the API returns the full result.
        /// </summary>
        /// <param name="prompt">The prompt to generate from</param>
        /// <param name="prompts">The prompts to generate from</param>
        /// <param name="suffix">The suffix that comes after a completion of inserted text.</param>
        /// <param name="maxTokens">How many tokens to complete to. Can return fewer if a stop sequence is hit.</param>
        /// <param name="temperature">What sampling temperature to use. Higher values means the model will take more risks.
        /// Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer.
        /// It is generally recommend to use this or <paramref name="topP"/> but not both.</param>
        /// <param name="topP">An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens
        /// comprising the top 10% probability mass are considered. It is generally recommend to use this or
        /// <paramref name="temperature"/> but not both.</param>
        /// <param name="numOutputs">How many different choices to request for each prompt.</param>
        /// <param name="presencePenalty">The scale of the penalty applied if a token is already present at all.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="frequencyPenalty">The scale of the penalty for how often a token is used.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="logProbabilities">Include the log probabilities on the logprobs most likely tokens, which can be found
        /// in <see cref="CompletionResult.Completions"/> -> <see cref="Choice.LogProbabilities"/>. So for example, if logprobs is 10,
        /// the API will return a list of the 10 most likely tokens. If logprobs is supplied, the API will always return the logprob
        /// of the sampled token, so there may be up to logprobs+1 elements in the response.</param>
        /// <param name="echo">Echo back the prompt in addition to the completion.</param>
        /// <param name="stopSequences">One or more sequences where the API will stop generating further tokens.
        /// The returned text will not contain the stop sequence.</param>
        /// <param name="model">Optional, <see cref="Model"/> to use when calling the API.
        /// Defaults to <see cref="Model.Davinci"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Asynchronously returns the completion result.
        /// Look in its <see cref="CompletionResult.Completions"/> property for the completions.</returns>
        public async Task<CompletionResult> CreateCompletionAsync(
            string prompt = null,
            IEnumerable<string> prompts = null,
            string suffix = null,
            int? maxTokens = null,
            double? temperature = null,
            double? topP = null,
            int? numOutputs = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            int? logProbabilities = null,
            bool? echo = null,
            IEnumerable<string> stopSequences = null,
            string model = null,
            CancellationToken cancellationToken = default)
        {
            var request = new CompletionRequest(
                string.IsNullOrWhiteSpace(model) ? Model.Davinci : model,
                prompt,
                prompts,
                suffix,
                maxTokens,
                temperature,
                topP,
                numOutputs,
                presencePenalty,
                frequencyPenalty,
                logProbabilities,
                echo,
                stopSequences);

            return await CreateCompletionAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified request.
        /// This is non-streaming, so it will wait until the API returns the full result.
        /// </summary>
        /// <param name="completionRequest">The request to send to the API.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Asynchronously returns the completion result.
        /// Look in its <see cref="CompletionResult.Completions"/> property for the completions.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<CompletionResult> CreateCompletionAsync(CompletionRequest completionRequest, CancellationToken cancellationToken = default)
        {
            completionRequest.Stream = false;
            var jsonContent = JsonSerializer.Serialize(completionRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.DeserializeResponse<CompletionResult>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        #endregion Non-Streaming

        #region Streaming

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified request,
        /// and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="resultHandler">An action to be called as each new result arrives.</param>
        /// <param name="prompt">The prompt to generate from</param>
        /// <param name="prompts">The prompts to generate from</param>
        /// <param name="suffix">The suffix that comes after a completion of inserted text.</param>
        /// <param name="maxTokens">How many tokens to complete to. Can return fewer if a stop sequence is hit.</param>
        /// <param name="temperature">What sampling temperature to use. Higher values means the model will take more risks.
        /// Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer.
        /// It is generally recommend to use this or <paramref name="topP"/> but not both.</param>
        /// <param name="topP">An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens
        /// comprising the top 10% probability mass are considered. It is generally recommend to use this
        /// or <paramref name="temperature"/> but not both.</param>
        /// <param name="numOutputs">How many different choices to request for each prompt.</param>
        /// <param name="presencePenalty">The scale of the penalty applied if a token is already present at all.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="frequencyPenalty">The scale of the penalty for how often a token is used.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="logProbabilities">Include the log probabilities on the logProbabilities most likely tokens,
        /// which can be found in <see cref="CompletionResult.Completions"/> -> <see cref="Choice.LogProbabilities"/>.
        /// So for example, if logProbabilities is 10, the API will return a list of the 10 most likely tokens.
        /// If logProbabilities is supplied, the API will always return the logProbabilities of the sampled token,
        /// so there may be up to logProbabilities+1 elements in the response.</param>
        /// <param name="echo">Echo back the prompt in addition to the completion.</param>
        /// <param name="stopSequences">One or more sequences where the API will stop generating further tokens.
        /// The returned text will not contain the stop sequence.</param>
        /// <param name="model">Optional, <see cref="Model"/> to use when calling the API.
        /// Defaults to <see cref="Model.Davinci"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>An async enumerable with each of the results as they come in.
        /// See <see href="https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#asynchronous-streams">the C# docs</see>
        /// for more details on how to consume an async enumerable.</returns>
        public async Task StreamCompletionAsync(
            Action<CompletionResult> resultHandler,
            string prompt = null,
            IEnumerable<string> prompts = null,
            string suffix = null,
            int? maxTokens = null,
            double? temperature = null,
            double? topP = null,
            int? numOutputs = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            int? logProbabilities = null,
            bool? echo = null,
            IEnumerable<string> stopSequences = null,
            string model = null,
            CancellationToken cancellationToken = default)
        {
            var request = new CompletionRequest(
                string.IsNullOrWhiteSpace(model) ? Model.Davinci : model,
                prompt,
                prompts,
                suffix,
                maxTokens,
                temperature,
                topP,
                numOutputs,
                presencePenalty,
                frequencyPenalty,
                logProbabilities,
                echo,
                stopSequences);

            await StreamCompletionAsync(request, resultHandler, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified request,
        /// and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="completionRequest">The request to send to the API.</param>
        /// <param name="resultHandler">An action to be called as each new result arrives.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task StreamCompletionAsync(CompletionRequest completionRequest, Action<CompletionResult> resultHandler, CancellationToken cancellationToken = default)
        {
            completionRequest.Stream = true;
            var jsonContent = JsonSerializer.Serialize(completionRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            using var request = new HttpRequestMessage(HttpMethod.Post, GetUrl());
            request.Content = jsonContent;
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } streamData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (streamData.TryGetEventStreamData(out var eventData))
                {
                    if (string.IsNullOrWhiteSpace(eventData)) { continue; }

                    resultHandler(response.DeserializeResponse<CompletionResult>(eventData, OpenAIClient.JsonSerializationOptions));
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified request, and stream the results as they come in.<br/>
        /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
        /// you may need to use <see cref="StreamCompletionAsync(CompletionRequest, Action{CompletionResult}, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="prompt">The prompt to generate from</param>
        /// <param name="prompts">The prompts to generate from</param>
        /// <param name="suffix">The suffix that comes after a completion of inserted text.</param>
        /// <param name="maxTokens">How many tokens to complete to. Can return fewer if a stop sequence is hit.</param>
        /// <param name="temperature">What sampling temperature to use. Higher values means the model will take more risks.
        /// Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer.
        /// It is generally recommend to use this or <paramref name="topP"/> but not both.</param>
        /// <param name="topP">An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens
        /// comprising the top 10% probability mass are considered. It is generally recommend to use this
        /// or <paramref name="temperature"/> but not both.</param>
        /// <param name="numOutputs">How many different choices to request for each prompt.</param>
        /// <param name="presencePenalty">The scale of the penalty applied if a token is already present at all.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="frequencyPenalty">The scale of the penalty for how often a token is used.
        /// Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.</param>
        /// <param name="logProbabilities">Include the log probabilities on the logProbabilities most likely tokens,
        /// which can be found in <see cref="CompletionResult.Completions"/> -> <see cref="Choice.LogProbabilities"/>.
        /// So for example, if logProbabilities is 10, the API will return a list of the 10 most likely tokens.
        /// If logProbabilities is supplied, the API will always return the logProbabilities of the sampled token,
        /// so there may be up to logProbabilities+1 elements in the response.</param>
        /// <param name="echo">Echo back the prompt in addition to the completion.</param>
        /// <param name="stopSequences">One or more sequences where the API will stop generating further tokens.
        /// The returned text will not contain the stop sequence.</param>
        /// <param name="model">Optional, <see cref="Model"/> to use when calling the API.
        /// Defaults to <see cref="Model.Davinci"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>An async enumerable with each of the results as they come in.
        /// See <see href="https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#asynchronous-streams">the C# docs</see>
        /// for more details on how to consume an async enumerable.</returns>
        public IAsyncEnumerable<CompletionResult> StreamCompletionEnumerableAsync(
            string prompt = null,
            IEnumerable<string> prompts = null,
            string suffix = null,
            int? maxTokens = null,
            double? temperature = null,
            double? topP = null,
            int? numOutputs = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            int? logProbabilities = null,
            bool? echo = null,
            IEnumerable<string> stopSequences = null,
            string model = null,
            CancellationToken cancellationToken = default)
        {
            var request = new CompletionRequest(
                string.IsNullOrWhiteSpace(model) ? Model.Davinci : model,
                prompt,
                prompts,
                suffix,
                maxTokens,
                temperature,
                topP,
                numOutputs,
                presencePenalty,
                frequencyPenalty,
                logProbabilities,
                echo,
                stopSequences);

            return StreamCompletionEnumerableAsync(request, cancellationToken);
        }

        /// <summary>
        /// Ask the API to complete the prompt(s) using the specified request, and stream the results as they come in.<br/>
        /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
        /// you may need to use <see cref="StreamCompletionAsync(CompletionRequest, Action{CompletionResult}, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="completionRequest">The request to send to the API.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>An async enumerable with each of the results as they come in.
        /// See <seealso href="https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#asynchronous-streams"/>
        /// for more details on how to consume an async enumerable.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async IAsyncEnumerable<CompletionResult> StreamCompletionEnumerableAsync(CompletionRequest completionRequest, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            completionRequest.Stream = true;
            var jsonContent = JsonSerializer.Serialize(completionRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            using var request = new HttpRequestMessage(HttpMethod.Post, GetUrl());
            request.Content = jsonContent;
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } streamData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (streamData.TryGetEventStreamData(out var eventData))
                {
                    if (string.IsNullOrWhiteSpace(eventData)) { continue; }
                    yield return response.DeserializeResponse<CompletionResult>(eventData, OpenAIClient.JsonSerializationOptions);
                }
                else
                {
                    break;
                }
            }
        }

        #endregion Streaming
    }
}
