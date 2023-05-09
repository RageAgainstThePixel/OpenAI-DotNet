using System.Net.Http;
using System.Threading.Tasks;
using OpenAI.Models;

namespace OpenAI.Moderations;

public interface IModerationsEndpoint
{
    /// <summary>
    /// Classifies if text violates OpenAI's Content Policy.
    /// </summary>
    /// <param name="input">
    /// The input text to classify.
    /// </param>
    /// <param name="model">The default is text-moderation-latest which will be automatically upgraded over time.
    /// This ensures you are always using our most accurate model.
    /// If you use text-moderation-stable, we will provide advanced notice before updating the model.
    /// Accuracy of text-moderation-stable may be slightly lower than for text-moderation-latest.
    /// </param>
    /// <returns>
    /// True, if the text has been flagged by the model as violating OpenAI's content policy.
    /// </returns>
    Task<bool> GetModerationAsync(string input, Model model = null);

    /// <summary>
    /// Classifies if text violates OpenAI's Content Policy
    /// </summary>
    /// <param name="request"><see cref="ModerationsRequest"/></param>
    /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
    Task<ModerationsResponse> CreateModerationAsync(ModerationsRequest request);
}