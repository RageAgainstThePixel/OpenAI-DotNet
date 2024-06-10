// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    /// <summary>
    /// The expiration policy for a vector store.
    /// </summary>
    public sealed class ExpirationPolicy
    {
        public static implicit operator ExpirationPolicy(int days) => new(days);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="days">
        /// The number of days after the anchor time that the vector store will expire.
        /// </param>
        public ExpirationPolicy(int days)
        {
            Days = days;
        }

        /// <summary>
        /// Anchor timestamp after which the expiration policy applies.
        /// Supported anchors: 'last_active_at'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("anchor")]
        public string Anchor { get; private set; } = "last_active_at";

        /// <summary>
        /// The number of days after the anchor time that the vector store will expire.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("days")]
        public int Days { get; private set; }
    }
}
