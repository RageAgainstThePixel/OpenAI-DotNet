namespace OpenAI
{

    public class RateLimits
    {

        /// <summary>
        /// The maximum number of requests that are permitted before exhausting the rate limit.
        /// </summary>
        public int LimitRequests { get; set; }

        /// <summary>
        /// The maximum number of tokens that are permitted before exhausting the rate limit.
        /// </summary>
        public int LimitTokens { get; set; }

        /// <summary>
        /// The remaining number of requests that are permitted before exhausting the rate limit.
        /// </summary>
        public int RemainingRequests { get; set; }

        /// <summary>
        /// The remaining number of tokens that are permitted before exhausting the rate limit.
        /// </summary>
        public int RemainingTokens { get; set; }

        /// <summary>
        /// The time until the rate limit (based on requests) resets to its initial state.
        /// </summary>
        public string ResetRequests { get; set; }

        /// <summary>
        /// The time until the rate limit (based on tokens) resets to its initial state.
        /// </summary>
        public string ResetTokens { get; set; }
    }
}