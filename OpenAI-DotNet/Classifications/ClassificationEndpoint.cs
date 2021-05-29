using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI_DotNet
{
    /// <summary>
    /// Classifies the specified query using provided examples.
    /// The endpoint first searches over the labeled examples to select the ones most relevant for the particular query.
    /// Then, the relevant examples are combined with the query to construct a prompt to produce the final label via the completions endpoint.
    /// <see href="https://beta.openai.com/docs/guides/classifications"/>
    /// </summary>
    public class ClassificationEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        internal ClassificationEndpoint(OpenAI api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint(Engine engine = null)
        {
            return $"{Api.BaseUrl}classifications";
        }

        /// <summary>
        /// Given a query and a set of labeled examples, the model will predict the most likely label for the query.
        /// </summary>
        /// <param name="request">The <see cref="ClassificationRequest"/> to use for the query.</param>
        /// <returns>A <see cref="ClassificationResponse"/>.</returns>
        public async Task<ClassificationResponse> GetClassificationAsync(ClassificationRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request, Api.JsonSerializationOptions);
            var response = await Api.Client.PostAsync(GetEndpoint(), jsonContent.ToJsonStringContent());

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ClassificationResponse>(await response.Content.ReadAsStringAsync());
                result.SetResponseData(response.Headers);

                return result;
            }

            throw new HttpRequestException($"{nameof(GetClassificationAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {jsonContent}");
        }
    }
}