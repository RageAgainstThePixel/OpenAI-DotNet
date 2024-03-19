// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        private static readonly NumberFormatInfo numberFormatInfo = new()
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

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse, HttpContent requestContent = null, MemoryStream responseStream = null, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var debugMessage = new StringBuilder();

            if (!response.IsSuccessStatusCode || debugResponse)
            {
                if (!string.IsNullOrWhiteSpace(methodName))
                {
                    debugMessage.Append($"{methodName} -> ");
                }

                var debugMessageObject = new Dictionary<string, Dictionary<string, object>>();

                if (response.RequestMessage != null)
                {
                    debugMessage.Append($"[{response.RequestMessage.Method}:{(int)response.StatusCode}] {response.RequestMessage.RequestUri}\n");

                    debugMessageObject["Request"] = new Dictionary<string, object>
                    {
                        ["Headers"] = response.RequestMessage.Headers.ToDictionary(pair => pair.Key, pair => pair.Value),
                    };
                }

                if (requestContent != null)
                {
                    debugMessageObject["Request"]["Body-Headers"] = requestContent.Headers.ToDictionary(pair => pair.Key, pair => pair.Value);
                    string requestAsString;

                    if (requestContent is MultipartFormDataContent multipartFormData)
                    {
                        var stringContents = multipartFormData.Select<HttpContent, object>(content =>
                        {
                            var headers = content.Headers.ToDictionary(pair => pair.Key, pair => pair.Value);
                            switch (content)
                            {
                                case StringContent stringContent:
                                    var valueAsString = stringContent.ReadAsStringAsync(cancellationToken).Result;
                                    object value;

                                    try
                                    {
                                        value = JsonNode.Parse(valueAsString);
                                    }
                                    catch
                                    {
                                        value = valueAsString;
                                    }

                                    return new { headers, value };
                                default:
                                    return new { headers };
                            }
                        });
                        requestAsString = JsonSerializer.Serialize(stringContents);
                    }
                    else
                    {
                        requestAsString = await requestContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    }

                    if (!string.IsNullOrWhiteSpace(requestAsString))
                    {
                        try
                        {
                            debugMessageObject["Request"]["Body"] = JsonNode.Parse(requestAsString);
                        }
                        catch
                        {
                            debugMessageObject["Request"]["Body"] = requestAsString;
                        }
                    }
                }

                debugMessageObject["Response"] = new()
                {
                    ["Headers"] = response.Headers.ToDictionary(pair => pair.Key, pair => pair.Value),
                };

                if (responseStream != null || !string.IsNullOrWhiteSpace(responseAsString))
                {
                    debugMessageObject["Response"]["Body"] = new Dictionary<string, object>();
                }

                if (responseStream != null)
                {
                    var body = Encoding.UTF8.GetString(responseStream.ToArray());

                    try
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Stream"] = JsonNode.Parse(body);
                    }
                    catch
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Stream"] = body;
                    }
                }

                if (!string.IsNullOrWhiteSpace(responseAsString))
                {
                    try
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Content"] = JsonNode.Parse(responseAsString);
                    }
                    catch
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Content"] = responseAsString;
                    }
                }

                debugMessage.Append(JsonSerializer.Serialize(debugMessageObject, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine(debugMessage.ToString());
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(message: $"{methodName} Failed! HTTP status code: {response.StatusCode} | Response body: {responseAsString}", null, statusCode: response.StatusCode);
            }

            return responseAsString;
        }

        internal static async Task CheckResponseAsync(this HttpResponseMessage response, bool debug, StringContent requestContent = null, MemoryStream responseStream = null, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            if (!response.IsSuccessStatusCode || debug)
            {
                await response.ReadAsStringAsync(debug, requestContent, responseStream, cancellationToken, methodName).ConfigureAwait(false);
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
