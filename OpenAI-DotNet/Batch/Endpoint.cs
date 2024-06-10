// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Batch
{
    public sealed class Endpoint
    {
        public const string ChatCompletions = "/v1/chat/completions";
        public const string Embeddings = "/v1/embeddings";
        public const string Completions = "/v1/completions";

        public Endpoint(string endpoint) => Value = endpoint;

        public string Value { get; }

        public override string ToString() => Value;

        public static implicit operator string(Endpoint endpoint) => endpoint?.ToString();

        public static implicit operator Endpoint(string endpoint) => new(endpoint);
    }
}
