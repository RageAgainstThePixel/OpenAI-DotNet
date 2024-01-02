// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

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
        /// The request id of this API call, as reported in the response headers.  This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
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
        /// The time until the rate limit (based on tokens) resets to its initial state.
        /// </summary>
        [JsonIgnore]
        public string ResetTokens { get; internal set; }
    }
}
