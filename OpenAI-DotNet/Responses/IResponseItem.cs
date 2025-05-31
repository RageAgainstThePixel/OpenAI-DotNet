// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Responses
{
    public interface IResponseItem : IListItem
    {
        /// <summary>
        /// The unique ID of this response item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The type of response item.
        /// </summary>
        public ResponseItemType Type { get; }

        public string Object { get; }

        /// <summary>
        /// The status of the response item.
        /// </summary>
        public ResponseStatus Status { get; }
    }
}
