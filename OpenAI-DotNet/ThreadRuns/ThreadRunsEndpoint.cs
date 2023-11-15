using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Extensions;
using OpenAI.ThreadRuns;

namespace OpenAI.ThreadMessages;

public class ThreadRunsEndpoint : BaseEndPoint
{
    public ThreadRunsEndpoint(OpenAIClient api) : base(api)
    {
    }

    protected override string Root => "threads";

    /// <summary>
    /// Create a run.
    /// </summary>
    /// <param name="threadId">The ID of the thread to run.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A run object.</returns>
    public async Task<ThreadRun> CreateThreadRunAsync(string threadId, CreateThreadRunRequest request,
        CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions)
            .ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs"), jsonContent, cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }

    /// <summary>
    /// Retrieves a run.
    /// </summary>
    /// <param name="threadId">The ID of the thread that was run.</param>
    /// <param name="runId">The ID of the run to retrieve.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The run object matching the specified ID.</returns>
    public async Task<ThreadRun> RetrieveRunAsync(string threadId, string runId,
        CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}"), cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var run = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return run;
    }

    /// <summary>
    /// Modifies a run.
    /// </summary>
    /// <param name="threadId">The ID of the thread that was run.</param>
    /// <param name="runId">The ID of the run to modify.</param>
    /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
    /// This can be useful for storing additional information about the object in a structured format.
    /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The modified run object matching the specified ID.</returns>
    public async Task<ThreadRun> ModifyThreadRunAsync(string threadId, string runId,
        Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(new { metadata = metadata }, OpenAIClient.JsonSerializationOptions)
            .ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}"), jsonContent, cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }

    /// <summary>
    /// Returns a list of runs belonging to a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread the run belongs to.</param>
    /// <param name="limit">A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.</param>
    /// <param name="order">Sort order by the created_at timestamp of the objects. asc for ascending order and desc for descending order.</param>
    /// <param name="after">A cursor for use in pagination. after is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.</param>
    /// <param name="before">A cursor for use in pagination. before is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of run objects.</returns>
    public async Task<ThreadRunsList> ListThreadRunsAsync(
        string threadId, int? limit = null, string order = "desc", string after = null, string before = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();
        if (limit.HasValue) parameters.Add("limit", limit.ToString());
        if (!String.IsNullOrEmpty(order)) parameters.Add("order", order);
        if (!String.IsNullOrEmpty(after)) parameters.Add("after", after);
        if (!String.IsNullOrEmpty(before)) parameters.Add("before", before);

        var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs", parameters), cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var runs = JsonSerializer.Deserialize<ThreadRunsList>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return runs;
    }

    /// <summary>
    /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
    /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
    /// All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="threadId">The ID of the thread to which this run belongs.</param>
    /// <param name="runId">The ID of the run that requires the tool output submission.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The modified run object matching the specified ID.</returns>
    public async Task<ThreadRun> SubmitToolOutputsAsync(
        string threadId,
        string runId,
        SubmitThreadRunToolOutputsRequest request,
        CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions)
            .ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(
                GetUrl($"/{threadId}/runs/{runId}/submit_tool_outputs"),
                jsonContent,
                cancellationToken)
            .ConfigureAwait(false);

        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var run = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return run;
    }

    /// <summary>
    /// Cancels a run that is in_progress.
    /// </summary>
    /// <param name="threadId">The ID of the thread to which this run belongs.</param>
    /// <param name="runId">The ID of the run to cancel.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The modified run object matching the specified ID.</returns>
    public async Task<ThreadRun> CancelThreadRunAsync(string threadId, string runId,
        CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/cancel"), content: null, cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }
    
    /// <summary>
    /// Create a thread and run it in one request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A run object.</returns>
    public async Task<ThreadRun> CreateThreadAndRunAsync(CreateThreadAndRunRequest request, CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions)
            .ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl($"/runs"), jsonContent, cancellationToken)
            .ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }
}