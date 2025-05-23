// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Audio
{
    public sealed class ChunkingStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefixPaddingMs">
        /// Amount of audio to include before the VAD detected speech (in milliseconds).
        /// </param>
        /// <param name="silenceDurationMs">
        /// Duration of silence to detect speech stop (in milliseconds). With shorter values the model will respond more quickly, but may jump in on short pauses from the user.
        /// </param>
        /// <param name="threshold">
        /// Sensitivity threshold (0.0 to 1.0) for voice activity detection. A higher threshold will require louder audio to activate the model, and thus might perform better in noisy environments.
        /// </param>
        public ChunkingStrategy(int? prefixPaddingMs = null, int? silenceDurationMs = null, float? threshold = null)
        {
            Type = "server_vad";
            PrefixPaddingMs = prefixPaddingMs;
            SilenceDurationMs = silenceDurationMs;
            Threshold = threshold;
        }

        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// Amount of audio to include before the VAD detected speech (in milliseconds).
        /// </summary>
        [JsonPropertyName("prefix_padding_ms")]
        public int? PrefixPaddingMs { get; }

        /// <summary>
        /// Duration of silence to detect speech stop (in milliseconds). With shorter values the model will respond more quickly, but may jump in on short pauses from the user.
        /// </summary>
        [JsonPropertyName("silence_duration_ms")]
        public int? SilenceDurationMs { get; }

        /// <summary>
        /// Sensitivity threshold (0.0 to 1.0) for voice activity detection. A higher threshold will require louder audio to activate the model, and thus might perform better in noisy environments.
        /// </summary>
        [JsonPropertyName("threshold")]
        public float? Threshold { get; }

        public static ChunkingStrategy Auto => new()
        {
            Type = "auto"
        };
    }
}
