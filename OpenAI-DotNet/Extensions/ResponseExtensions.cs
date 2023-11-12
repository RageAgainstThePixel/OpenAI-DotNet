using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class ResponseExtensions
    {
        private const string RequestId = "X-Request-ID";
        private const string Organization = "Openai-Organization";
        private const string ProcessingTime = "Openai-Processing-Ms";
        private const string OpenAIVersion = "openai-version";
        private const string XRateLimitLimitRequests = "x-ratelimit-limit-requests";
        private const string XRateLimitLimitTokens = "x-ratelimit-limit-tokens";
        private const string XRateLimitRemainingRequests = "x-ratelimit-remaining-requests";
        private const string XRateLimitRemainingTokens = "x-ratelimit-remaining-tokens";
        private const string XRateLimitResetRequests = "x-ratelimit-reset-requests";
        private const string XRateLimitResetTokens = "x-ratelimit-reset-tokens";

        private static readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers)
        {
            if (headers == null) { return; }

            if (headers.TryGetValues(RequestId, out var requestId))
            {
                response.RequestId = requestId.First();
            }

            if (headers.TryGetValues(Organization, out var organization))
            {
                response.Organization = organization.First();
            }

            if (headers.TryGetValues(ProcessingTime, out var processingTimeString) &&
                double.TryParse(processingTimeString.First(), NumberStyles.AllowDecimalPoint, numberFormatInfo, out var processingTime))
            {
                response.ProcessingTime = TimeSpan.FromMilliseconds(processingTime);
            }

            if (headers.TryGetValues(OpenAIVersion, out var version))
            {
                response.OpenAIVersion = version.First();
            }

            if (headers.TryGetValues(XRateLimitLimitRequests, out var limitRequests) &&
                int.TryParse(limitRequests.FirstOrDefault(), out var limitRequestsValue)
               )
            {
                response.LimitRequests = limitRequestsValue;
            }

            if (headers.TryGetValues(XRateLimitLimitTokens, out var limitTokens) &&
                int.TryParse(limitTokens.FirstOrDefault(), out var limitTokensValue))
            {
                response.LimitTokens = limitTokensValue;
            }

            if (headers.TryGetValues(XRateLimitRemainingRequests, out var remainingRequests) &&
                int.TryParse(remainingRequests.FirstOrDefault(), out var remainingRequestsValue))
            {
                response.RemainingRequests = remainingRequestsValue;
            }

            if (headers.TryGetValues(XRateLimitRemainingTokens, out var remainingTokens) &&
                int.TryParse(remainingTokens.FirstOrDefault(), out var remainingTokensValue))
            {
                response.RemainingTokens = remainingTokensValue;
            }

            if (headers.TryGetValues(XRateLimitResetRequests, out var resetRequests))
            {
                response.ResetRequests = resetRequests.FirstOrDefault();
            }

            if (headers.TryGetValues(XRateLimitResetTokens, out var resetTokens))
            {
                response.ResetTokens = resetTokens.FirstOrDefault();
            }
        }

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse = false, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(message: $"{methodName} Failed! HTTP status code: {response.StatusCode} | Response body: {responseAsString}", null, statusCode: response.StatusCode);
            }

            if (debugResponse)
            {
                Console.WriteLine(responseAsString);
            }

            return responseAsString;
        }

        internal static async Task CheckResponseAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new HttpRequestException(message: $"{methodName} Failed! HTTP status code: {response.StatusCode} | Response body: {responseAsString}", null, statusCode: response.StatusCode);
            }
        }

        internal static T DeserializeResponse<T>(this HttpResponseMessage response, string json, JsonSerializerOptions settings) where T : BaseResponse
        {
            var result = JsonSerializer.Deserialize<T>(json, settings);
            result.SetResponseData(response.Headers);
            return result;
        }
    }
}
