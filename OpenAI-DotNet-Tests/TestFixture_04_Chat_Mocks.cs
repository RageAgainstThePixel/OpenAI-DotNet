// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_04_Chat_Mocks
    {
        [Test]
        public async Task Test_01_01_Azure_Streaming_Ignore_Empty_Ids()
        {
            var httpClient = MockHttpClient(@"data: {""choices"":[],""created"":0,""id"":"""",""model"":"""",""object"":"""",""prompt_filter_results"":[{""prompt_index"":0,""content_filter_results"":{""hate"":{""filtered"":false,""severity"":""safe""},""self_harm"":{""filtered"":false,""severity"":""safe""},""sexual"":{""filtered"":false,""severity"":""safe""},""violence"":{""filtered"":false,""severity"":""safe""}}}]}

data: {""choices"":[{""delta"":{""content"":"""",""role"":""assistant""},""finish_reason"":null,""index"":0}],""created"":1723623074,""id"":""chatcmpl-foobar"",""model"":""gpt-4o-2024-05-13"",""object"":""chat.completion.chunk"",""system_fingerprint"":""fp_foobar""}

data: {""choices"":[{""delta"":{""content"":""Hello""},""finish_reason"":null,""index"":0}],""created"":1723623074,""id"":""chatcmpl-foobar"",""model"":""gpt-4o-2024-05-13"",""object"":""chat.completion.chunk"",""system_fingerprint"":""fp_foobar""}

data: {""choices"":[{""delta"":{""content"":""!""},""finish_reason"":null,""index"":0}],""created"":1723623074,""id"":""chatcmpl-foobar"",""model"":""gpt-4o-2024-05-13"",""object"":""chat.completion.chunk"",""system_fingerprint"":""fp_foobar""}

data: {""choices"":[{""delta"":{},""finish_reason"":""stop"",""index"":0}],""created"":1723623074,""id"":""chatcmpl-foobar"",""model"":""gpt-4o-2024-05-13"",""object"":""chat.completion.chunk"",""system_fingerprint"":""fp_foobar""}

data: {""choices"":[{""content_filter_offsets"":{""check_offset"":78,""start_offset"":78,""end_offset"":112},""content_filter_results"":{""hate"":{""filtered"":false,""severity"":""safe""},""self_harm"":{""filtered"":false,""severity"":""safe""},""sexual"":{""filtered"":false,""severity"":""safe""},""violence"":{""filtered"":false,""severity"":""safe""}},""finish_reason"":null,""index"":0}],""created"":0,""id"":"""",""model"":"""",""object"":""""}

data: [DONE]

");
            var openaiClient = new OpenAIClient(new OpenAIAuthentication("sk-foobar"), client: httpClient);

            var messages = new List<Message>
            {
                new(Role.System, "Say hello world"),
                new(Role.User, "Hi")
            };

            var request = new ChatRequest(messages);

            var receivedAnyChunk = false;
            await foreach (var chunk in openaiClient.ChatEndpoint.StreamCompletionEnumerableAsync(request))
            {
                Assert.NotNull(chunk);
                receivedAnyChunk = true;
            }
            Assert.IsTrue(receivedAnyChunk);
        }

        private static HttpClient MockHttpClient(string mockedResponseContent)
        {
            var mockedResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockedResponse.Content = new StringContent(mockedResponseContent);
            var fakeHandler = new MockedHttpMessageHandler(mockedResponse);
            return new HttpClient(fakeHandler) { BaseAddress = new Uri("http://localhost") };
        }

        private class MockedHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage response;

            public MockedHttpMessageHandler(HttpResponseMessage response)
            {
                this.response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(response);
            }
        }
    }
}
