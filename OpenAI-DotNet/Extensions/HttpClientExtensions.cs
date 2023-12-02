#nullable enable
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class HttpClientExtensions
    {
        internal static Task<HttpResponseMessage> GetResponseAsync(
            this HttpClient client,
            string url,
            HttpMethod httpMethod,
            HttpContent? httpContent = null,
            CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(httpMethod, url)
            {
                Content = httpContent
            };

            return client.GetResponseAsync(request, cancellationToken: cancellationToken);
        }

        internal static async Task<HttpResponseMessage> GetResponseAsync(
            this HttpClient client,
            HttpRequestMessage request,
            HttpCompletionOption completionOptions = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request, completionOptions, cancellationToken).ConfigureAwait(false);
                await response.ThrowApiExceptionIfUnsuccessfulAsync(cancellationToken).ConfigureAwait(false);
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new OpenAITimeoutException(ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new OpenAITimeoutException(ex);
            }
        }
    }
}
