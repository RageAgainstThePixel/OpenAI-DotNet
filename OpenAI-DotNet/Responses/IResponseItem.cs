// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Responses
{
    public interface IResponseItem : IServerSentEvent, IListItem
    {
        /// <summary>
        /// The unique ID of this response item.
        /// </summary>
        string Id { get; }

        ResponseItemType Type { get; }

        /// <summary>
        /// The status of the response item.
        /// </summary>
        ResponseStatus Status { get; }
    }
}
