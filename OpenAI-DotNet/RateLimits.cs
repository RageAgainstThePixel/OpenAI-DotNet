namespace OpenAI;

public class RateLimits
{

    /*
     *
       FIELD	SAMPLE VALUE	DESCRIPTION
       x-ratelimit-limit-requests	60	The maximum number of requests that are permitted before exhausting the rate limit.
       x-ratelimit-limit-tokens	150000	The maximum number of tokens that are permitted before exhausting the rate limit.
       x-ratelimit-remaining-requests	59	The remaining number of requests that are permitted before exhausting the rate limit.
       x-ratelimit-remaining-tokens	149984	The remaining number of tokens that are permitted before exhausting the rate limit.
       x-ratelimit-reset-requests	1s	The time until the rate limit (based on requests) resets to its initial state.
       x-ratelimit-reset-tokens	6m0s	The time until the rate limit (based on tokens) resets to its initial state.
     */

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