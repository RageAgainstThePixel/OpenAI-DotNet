// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace OpenAI.Responses
{
    public sealed class ResponsesEndpoint : OpenAIBaseEndpoint
    {
        public ResponsesEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "responses";
    }
}
