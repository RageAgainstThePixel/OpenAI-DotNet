using OpenAI.Extensions;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Edits
{
    /// <summary>
    /// Given a prompt and an instruction, the model will return an edited version of the prompt.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/edits"/>
    /// </summary>
    public sealed class EditsEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public EditsEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "edits";

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
        public async Task<string> CreateEditAsync(
            string input,
            string instruction,
            int? editCount,
            double? temperature,
            double? topP,
            string model = null)
        {
            var request = new EditRequest(input, instruction, editCount, temperature, topP, model);
            var result = await CreateEditAsync(request).ConfigureAwait(false);
            return result.ToString();
        }

        /// <summary>
        /// Creates a new edit for the provided input, instruction, and parameters
        /// </summary>
        /// <param name="request"><see cref="EditRequest"/></param>
        /// <returns><see cref="EditResponse"/></returns>
        public async Task<EditResponse> CreateEditAsync(EditRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request, Api.JsonSerializationOptions).ToJsonStringContent();
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return response.DeserializeResponse<EditResponse>(responseAsString, Api.JsonSerializationOptions);
        }
    }
}
