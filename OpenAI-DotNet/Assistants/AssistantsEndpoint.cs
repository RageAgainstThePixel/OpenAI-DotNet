using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Extensions;

namespace OpenAI.Assistants;

public class AssistantsEndpoint : BaseEndPoint
{
    internal AssistantsEndpoint(OpenAIClient api) : base(api) { }

    protected override string Root => "assistants";
    
    /// <summary>
    /// Create an assistant with a model and instructions.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Assistant> CreateAssistantAsync(CreateAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<Assistant>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }

    /// <summary>
    /// Retrieves an assistant.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant to retrieve.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Assistant> RetrieveAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var assistant = JsonSerializer.Deserialize<Assistant>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return assistant;
    }
    
    /// <summary>
    /// Modifies an assistant.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant to modify.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Assistant> ModifyAssistantAsync(string assistantId, ModifyAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl($"/{assistantId}"), jsonContent, cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var modified = JsonSerializer.Deserialize<Assistant>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return modified;
    }

    /// <summary>
    /// Delete an assistant.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.DeleteAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var status = JsonSerializer.Deserialize<DeletionStatus>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return status.Deleted;
    }
    
    /// <summary>
    /// Get list of assistants.
    /// </summary>
    /// <param name="limit">A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.</param>
    /// <param name="order">Sort order by the created_at timestamp of the objects. asc for ascending order and desc for descending order.</param>
    /// <param name="after">A cursor for use in pagination. after is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.</param>
    /// <param name="before">A cursor for use in pagination. before is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssistantsList> ListAssistantsAsync(
        int? limit = null, string order = "desc", string after = null, string before = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();
        if (limit.HasValue) parameters.Add("limit", limit.ToString());
        if (!String.IsNullOrEmpty(order)) parameters.Add("order", order);
        if (!String.IsNullOrEmpty(after)) parameters.Add("after", after);
        if (!String.IsNullOrEmpty(before)) parameters.Add("before", before);

        var response = await Api.Client.GetAsync(GetUrl(queryParameters: parameters), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var list = JsonSerializer.Deserialize<AssistantsList>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return list;
    }

    /// <summary>
    /// Create an assistant file by attaching a File to an assistant.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant for which to create a File.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssistantFile> CreateAssistantFileAsync(string assistantId, CreateAssistantFileRequest request, CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
        var response = await Api.Client.PostAsync(GetUrl($"/{assistantId}/files"), jsonContent, cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<AssistantFile>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }

    /// <summary>
    /// Retrieves an AssistantFile.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant who the file belongs to.</param>
    /// <param name="fileId">The ID of the file we're getting.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssistantFile> RetrieveAssistantFileAsync(string assistantId, string fileId, CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var created = JsonSerializer.Deserialize<AssistantFile>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return created;
    }

    /// <summary>
    /// Delete an assistant file.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant that the file belongs to.</param>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAssistantFileAsync(string assistantId, string fileId, CancellationToken cancellationToken = default)
    {
        var response = await Api.Client.DeleteAsync(GetUrl($"/{assistantId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var status = JsonSerializer.Deserialize<DeletionStatus>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return status.Deleted;
    }

    /// <summary>
    /// Returns a list of assistant files.
    /// </summary>
    /// <param name="assistantId">The ID of the assistant the file belongs to.</param>
    /// <param name="limit">A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.</param>
    /// <param name="order">Sort order by the created_at timestamp of the objects. asc for ascending order and desc for descending order.</param>
    /// <param name="after">A cursor for use in pagination. after is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.</param>
    /// <param name="before">A cursor for use in pagination. before is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
    /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssistantFilesList> ListAssistantFilesAsync(
        string assistantId,
        int? limit = null, string order = "desc", string after = null, string before = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();
        if (limit.HasValue) parameters.Add("limit", limit.ToString());
        if (!String.IsNullOrEmpty(order)) parameters.Add("order", order);
        if (!String.IsNullOrEmpty(after)) parameters.Add("after", after);
        if (!String.IsNullOrEmpty(before)) parameters.Add("before", before);

        var response = await Api.Client.GetAsync(GetUrl($"/{assistantId}/files", parameters), cancellationToken).ConfigureAwait(false);
        var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
        var list = JsonSerializer.Deserialize<AssistantFilesList>(responseAsString, OpenAIClient.JsonSerializationOptions);

        return list;
    }

    private sealed class DeletionStatus
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("deleted")]
        public bool Deleted { get; private set; }
    }
}