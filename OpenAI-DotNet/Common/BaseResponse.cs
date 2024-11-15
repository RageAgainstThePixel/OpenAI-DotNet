// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace OpenAI
{
    public abstract class BaseResponse
    {
        /// <summary>
        /// The <see cref="OpenAIClient"/> this response was generated from.
        /// </summary>
        [JsonIgnore]
        public OpenAIClient Client { get; internal set; }

        /// <summary>
        /// The server-side processing time as reported by the API.  This can be useful for debugging where a delay occurs.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ProcessingTime { get; internal set; }

        /// <summary>
        /// The organization associated with the API request, as reported by the API.
        /// </summary>
        [JsonIgnore]
        public string Organization { get; internal set; }

        /// <summary>
        /// The request id of this API call, as reported in the response headers.
        /// This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
        /// </summary>
        [JsonIgnore]
        public string RequestId { get; internal set; }

        /// <summary>
        /// The version of the API used to generate this response, as reported in the response headers.
        /// </summary>
        [JsonIgnore]
        public string OpenAIVersion { get; internal set; }

        /// <summary>
        /// The maximum number of requests that are permitted before exhausting the rate limit.
        /// </summary>
        [JsonIgnore]
        public int? LimitRequests { get; internal set; }

        /// <summary>
        /// The maximum number of tokens that are permitted before exhausting the rate limit.
        /// </summary>
        [JsonIgnore]
        public int? LimitTokens { get; internal set; }

        /// <summary>
        /// The remaining number of requests that are permitted before exhausting the rate limit.
        /// </summary>
        [JsonIgnore]
        public int? RemainingRequests { get; internal set; }

        /// <summary>
        /// The remaining number of tokens that are permitted before exhausting the rate limit.
        /// </summary>
        [JsonIgnore]
        public int? RemainingTokens { get; internal set; }

        /// <summary>
        /// The time until the rate limit (based on requests) resets to its initial state.
        /// </summary>
        [JsonIgnore]
        public string ResetRequests { get; internal set; }

        /// <summary>
        /// The time until the rate limit (based on requests) resets to its initial state represented as a TimeSpan.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ResetRequestsTimespan => ConvertTimestampToTimespan(ResetTokens);

        /// <summary>
        /// The time until the rate limit (based on tokens) resets to its initial state.
        /// </summary>
        [JsonIgnore]
        public string ResetTokens { get; internal set; }

        /// <summary>
        /// The time until the rate limit (based on tokens) resets to its initial state represented as a TimeSpan.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ResetTokensTimespan => ConvertTimestampToTimespan(ResetTokens);

        /*
        * Regex Notes:
        *  The gist of this regex is that it is searching for "timestamp segments", e.g. 1m or 144ms.
        *  Each segment gets matched into its respective named capture group, from which we further parse out the 
        *  digits. This allows us to take the string 6m45s99ms and insert the integers into a 
        *  TimeSpan object for easier use.
        *
        *  Regex Performance Notes, against 100k randomly generated timestamps:
        *  Average performance: 0.0003ms
        *  Best case: 0ms
        *  Worst Case: 15ms
        *  Total Time: 30ms
        *
        *  Inconsequential compute time
        */
        private readonly Regex timestampRegex = new Regex(@"^(?<h>\d+h)?(?<m>\d+m(?!s))?(?<s>\d+s)?(?<ms>\d+ms)?");

        /// <summary>
        /// Takes a timestamp received from a OpenAI response header and converts to a TimeSpan
        /// </summary>
        /// <param name="timestamp">The timestamp received from an OpenAI header, e.g. x-ratelimit-reset-tokens</param>
        /// <returns>A TimeSpan that represents the timestamp provided</returns>
        /// <exception cref="ArgumentException">Thrown if the provided timestamp is not in the expected format, or if the match is not successful.</exception>
        private TimeSpan ConvertTimestampToTimespan(string timestamp)
        {
            var match = timestampRegex.Match(timestamp);

            if (!match.Success)
            {
                throw new ArgumentException($"Could not parse timestamp header. '{timestamp}'.");
            }

            /*
             * Note about Hours in timestamps:
             *  I have not personally observed a timestamp with an hours segment (e.g. 1h30m15s1ms).
             *  Although their presence may not actually exist, we can still have this section in the parser, there is no
             *  negative impact for a missing hours segment because the capture groups are flagged as optional.
             */
            int.TryParse(match.Groups["h"]?.Value.Replace("h", string.Empty), out var h);
            int.TryParse(match.Groups["m"]?.Value.Replace("m", string.Empty), out var m);
            int.TryParse(match.Groups["s"]?.Value.Replace("s", string.Empty), out var s);
            int.TryParse(match.Groups["ms"]?.Value.Replace("ms", string.Empty), out var ms);
            return new TimeSpan(h, m, s) + TimeSpan.FromMilliseconds(ms);
        }

        public string ToJsonString()
            => this.ToEscapedJsonString<object>();
    }
}
