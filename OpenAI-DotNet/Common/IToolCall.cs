// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;

namespace OpenAI
{
    public interface IToolCall
    {
        public string CallId { get; }

        public string Name { get; }

        public JsonNode Arguments { get; }
    }
}
