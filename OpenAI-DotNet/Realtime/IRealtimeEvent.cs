// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Realtime
{
    public interface IRealtimeEvent
    {
        /// <summary>
        /// The unique ID of the server event.
        /// </summary>
        public string EventId { get; }

        public string Type { get; }

        public string ToJsonString();
    }

    public interface IClientEvent : IRealtimeEvent
    {
    }

    public interface IServerEvent : IRealtimeEvent
    {
    }

    internal interface IRealtimeEventStream
    {
        public bool IsDone { get; }

        public bool IsDelta { get; }
    }
}
