// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Models
{
    /// <summary>
    /// List and describe the various models available in the API.
    /// You can refer to the Models documentation to understand which models are available for certain endpoints: <see href="https://platform.openai.com/docs/models/model-endpoint-compatibility"/>.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/models"/>
    /// </summary>
    public sealed class ModelsEndpoint : OpenAIBaseEndpoint
    {
        private sealed class ModelsList
        {
            [JsonInclude]
            [JsonPropertyName("data")]
            public List<Model> Models { get; private set; }
        }

        /// <inheritdoc />
        public ModelsEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "models";

        /// <summary>
        /// List all models via the API
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Asynchronously returns the list of all <see cref="Model"/>s</returns>
        public async Task<IReadOnlyList<Model>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl(), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<ModelsList>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Models;
        }

        /// <summary>
        /// Get the details about a particular Model from the API
        /// </summary>
        /// <param name="id">The id/name of the model to get more details about</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Asynchronously returns the <see cref="Model"/> with all available properties</returns>
        public async Task<Model> GetModelDetailsAsync(string id, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<Model>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete a fine-tuned model. You must have the Owner role in your organization.
        /// </summary>
        /// <param name="modelId">The <see cref="Model"/> to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if fine-tuned model was successfully deleted.</returns>
        public async Task<bool> DeleteFineTuneModelAsync(string modelId, CancellationToken cancellationToken = default)
        {
            var model = await GetModelDetailsAsync(modelId, cancellationToken).ConfigureAwait(false);

            if (model == null ||
                string.IsNullOrWhiteSpace(model))
            {
                throw new Exception($"Failed to get {modelId} info!");
            }

            // Don't check ownership as API does it for us.

            try
            {
                using var response = await client.Client.DeleteAsync(GetUrl($"/{model.Id}"), cancellationToken).ConfigureAwait(false);
                var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
                return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("You have insufficient permissions for this operation. You need to be this role: Owner."))
                {
                    throw new UnauthorizedAccessException($"You have insufficient permissions for this operation. You need to be this role: Owner.\n{e}");
                }

                throw;
            }
        }
    }
}
