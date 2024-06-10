// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.VectorStores
{
    public enum ChunkingStrategyType
    {
        /// <summary>
        /// The default strategy.
        /// This strategy currently uses a 'max_chunk_size_tokens' of '800' and 'chunk_overlap_tokens' of '400'.
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto,
        /// <summary>
        /// This is returned when the chunking strategy is unknown.
        /// Typically, this is because the file was indexed before the 'chunking_strategy' concept was introduced in the API.
        /// </summary>
        [EnumMember(Value = "other")]
        Other,
        [EnumMember(Value = "static")]
        Static
    }
}
