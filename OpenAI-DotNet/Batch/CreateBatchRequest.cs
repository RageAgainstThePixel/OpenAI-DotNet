// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Batch
{
    public sealed class CreateBatchRequest
    {
        private const string DefaultCompletionWindow = "24h";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFileId">
        /// The ID of an uploaded file that contains requests for the new batch.
        /// Your input file must be formatted as a JSONL file, and must be uploaded with the purpose batch.
        /// The file can contain up to 50,000 requests, and can be up to 100 MB in size.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint to be used for all requests in the batch.
        /// Currently, '/v1/chat/completions', '/v1/embeddings', and '/v1/completions' are supported.
        /// Note that '/v1/embeddings' batches are also restricted to a maximum of 50,000 embedding inputs across all requests in the batch.
        /// </param>
        /// <param name="metadata">
        /// Optional custom metadata for the batch.
        /// </param>
        public CreateBatchRequest(string inputFileId, string endpoint, IReadOnlyDictionary<string, object> metadata = null)
        {
            InputFileId = inputFileId;
            Endpoint = endpoint;
            CompletionWindow = DefaultCompletionWindow;
            Metadata = metadata;
        }

        [JsonPropertyName("input_file_id")]
        public string InputFileId { get; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; }

        [JsonPropertyName("completion_window")]
        public string CompletionWindow { get; }

        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, object> Metadata { get; }
    }
}
