// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI
{
    /// <summary>
    /// Common interface for streaming events
    /// </summary>
    public interface IStreamEvent
    {
        string Object { get; }

        string ToJsonString();
    }
}
