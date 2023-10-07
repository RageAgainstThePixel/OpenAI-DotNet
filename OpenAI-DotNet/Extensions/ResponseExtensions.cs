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
        }

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(message: $"{methodName} Failed! HTTP status code: {response.StatusCode} | Response body: {responseAsString}", null, statusCode: response.StatusCode);
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
