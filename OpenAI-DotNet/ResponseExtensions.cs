using System;
using System.Linq;
using System.Net.Http.Headers;

namespace OpenAI
{
    internal static class ResponseExtensions
    {
        private const string Organization = "Openai-Organization";
        private const string RequestId = "X-Request-ID";
        private const string ProcessingTime = "Openai-Processing-Ms";

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers)
        {
            response.Organization = headers.GetValues(Organization).FirstOrDefault();
            response.RequestId = headers.GetValues(RequestId).FirstOrDefault();
            response.ProcessingTime = TimeSpan.FromMilliseconds(int.Parse(headers.GetValues(ProcessingTime).First()));
        }
    }
}
