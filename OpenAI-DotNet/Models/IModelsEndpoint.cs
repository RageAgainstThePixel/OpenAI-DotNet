using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Models;

public interface IModelsEndpoint
{
    /// <summary>
    /// List all models via the API
    /// </summary>
    /// <returns>Asynchronously returns the list of all <see cref="Model"/>s</returns>
    /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
    Task<IReadOnlyList<Model>> GetModelsAsync();

    /// <summary>
    /// Get the details about a particular Model from the API
    /// </summary>
    /// <param name="id">The id/name of the model to get more details about</param>
    /// <returns>Asynchronously returns the <see cref="Model"/> with all available properties</returns>
    /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
    Task<Model> GetModelDetailsAsync(string id);

    /// <summary>
    /// Delete a fine-tuned model. You must have the Owner role in your organization.
    /// </summary>
    /// <param name="modelId">The <see cref="Model"/> to delete.</param>
    /// <returns>True, if fine-tuned model was successfully deleted.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<bool> DeleteFineTuneModelAsync(string modelId);
}