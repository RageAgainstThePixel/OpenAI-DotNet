using System;
using System.Linq;
using System.Net.Http.Headers;

namespace OpenAI
{
    public static class ResponseExtensions
    {
        private const string Organization = "Openai-Organization";
        private const string RequestId = "X-Request-ID";
        private const string ProcessingTime = "Openai-Processing-Ms";

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers)
        {
            response.Organization = TryGetCaseInsensitive(headers, Organization);
            response.RequestId = TryGetCaseInsensitive(headers, RequestId);
            response.ProcessingTime = TimeSpan.FromMilliseconds(double.Parse(TryGetCaseInsensitive(headers, ProcessingTime)));
        }

        internal static string TryGetCaseInsensitive(HttpResponseHeaders headers,string key)
        {
            if(headers.TryGetValues(key,out var values))
            {
                return values.FirstOrDefault();
            } else
            {
                return headers.GetValues(key.ToLower()).FirstOrDefault();
            }
        }

        
    }
}