using System.Threading.Tasks;
using OpenAI.Models;

namespace OpenAI.Edits;

public interface IEditsEndpoint
{
    /// <summary>
    /// Creates a new edit for the provided input, instruction, and parameters
    /// </summary>
    /// <param name="input">The input text to use as a starting point for the edit.</param>
    /// <param name="instruction">The instruction that tells the model how to edit the prompt.</param>
    /// <param name="editCount">How many edits to generate for the input and instruction.</param>
    /// <param name="temperature">
    /// What sampling temperature to use. Higher values means the model will take more risks.
    /// Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer.
    /// We generally recommend altering this or top_p but not both.
    /// </param>
    /// <param name="topP">
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the
    /// results of the tokens with top_p probability mass.
    /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    /// We generally recommend altering this or temperature but not both.
    /// </param>
    /// <param name="model">ID of the model to use. Defaults to text-davinci-edit-001.</param>
    /// <returns>The top edit result choice.</returns>
    Task<string> CreateEditAsync(
        string input,
        string instruction,
        int? editCount,
        double? temperature,
        double? topP,
        Model model = null);

    /// <summary>
    /// Creates a new edit for the provided input, instruction, and parameters
    /// </summary>
    /// <param name="request"><see cref="EditRequest"/></param>
    /// <returns><see cref="EditResponse"/></returns>
    Task<EditResponse> CreateEditAsync(EditRequest request);
}