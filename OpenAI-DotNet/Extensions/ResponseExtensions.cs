using System;
using System.Globalization;
using System.Linq;
using System.Net;
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

        private static readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers, OpenAIClient client)
        {
            if (response is IListResponse<BaseResponse> listResponse)
            {
                foreach (var item in listResponse.Items)
                {
                    SetResponseData(item, headers, client);
                }
            }

            response.Client = client;

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
        }

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse = false, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            await ThrowApiExceptionIfUnsuccessfulAsync(response, cancellationToken);

            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (debugResponse)
            {
                Console.WriteLine(responseAsString);
            }

            return responseAsString;
        }

        internal static async Task ThrowApiExceptionIfUnsuccessfulAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var contentType = response.Content.Headers.ContentType;

                var errorResponse = contentType is not null && contentType.MediaType == "application/json"
                    ? JsonSerializer.Deserialize<ApiErrorResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)
                    : null;

                var errorMessage = errorResponse is null
                    ? string.IsNullOrWhiteSpace(responseAsString)
                        ? $"Error code: {response.StatusCode}"
                        : responseAsString
                    : $"Error code: {response.StatusCode} - {responseAsString}";

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new OpenAIBadRequestException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.Unauthorized:
                        throw new OpenAIAuthenticationException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.Forbidden:
                        throw new OpenAIPermissionDeniedException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.NotFound:
                        throw new OpenAINotFoundException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.Conflict:
                        throw new ConflictException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.UnprocessableEntity:
                        throw new UnprocessableEntityException(responseAsString, errorResponse?.Error, errorMessage);
                    case HttpStatusCode.TooManyRequests:
                        throw new RateLimitException(response.Headers, responseAsString, errorResponse?.Error);
                    default:
                        int statusCode = (int)response.StatusCode;
                        if (statusCode>= 500)
                        {
                            // Not sure if it makes sense to handle ApIError here, as it most definitely might be null.
                            // It'd be surprised if they're handling 5XX with user-friendly payloads.
                            // For sake of not assuming anything, I'm writing this like so for now.
                            throw new InternalServerException(statusCode, responseAsString, errorResponse?.Error);
                        }

                        throw new OpenAIStatusException(errorMessage, statusCode, responseAsString, errorResponse?.Error);
                }
            }
        }

        internal static T Deserialize<T>(this HttpResponseMessage response, string json, OpenAIClient client) where T : BaseResponse
        {
            var result = JsonSerializer.Deserialize<T>(json, OpenAIClient.JsonSerializationOptions);
            result.SetResponseData(response.Headers, client);
            return result;
        }
    }
}
