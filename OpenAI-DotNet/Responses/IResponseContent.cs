// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Responses
{
    public interface IResponseContent
    {
        public ResponseContentType Type { get; }
    }
}
