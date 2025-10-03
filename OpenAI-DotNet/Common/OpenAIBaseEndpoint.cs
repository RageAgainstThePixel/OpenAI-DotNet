// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public abstract class OpenAIBaseEndpoint
    {
        protected OpenAIBaseEndpoint(OpenAIClient client) => this.client = client;

        // ReSharper disable once InconsistentNaming
        protected readonly OpenAIClient client;

        private HttpClient HttpClient => client.Client;

        private bool enableDebug;

        /// <summary>
        /// Enables or disables the logging of all http responses of header and body information for this endpoint.<br/>
        /// WARNING! Enabling this in your production build, could potentially leak sensitive information!
        /// </summary>
        public bool EnableDebug
        {
            get => enableDebug || client.EnableDebug;
            set => enableDebug = value;
        }

        /// <summary>
        /// The root endpoint address.
        /// </summary>
        protected abstract string Root { get; }

        /// <summary>
        /// Indicates if the endpoint has an Azure Deployment.
        /// </summary>
        /// <remarks>
        /// If the endpoint is an Azure deployment, is true.
        /// If it is not an Azure deployment, is false.
        /// If it is not an Azure supported Endpoint, is null.
        /// </remarks>
        protected virtual bool? IsAzureDeployment => null;

        /// <summary>
        /// Custom headers for this endpoint
        /// </summary>
        internal virtual IReadOnlyDictionary<string, IEnumerable<string>> Headers => null;

        protected Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, cancellationToken);
        }

        protected Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = content;
            return SendAsync(message, cancellationToken);
        }

        protected Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content, CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = content;
            return SendAsync(message, cancellationToken);
        }

        protected Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendAsync(message, cancellationToken);
        }

        protected Task<Stream> GetStreamAsync(string uri, CancellationToken cancellationToken)
            => HttpClient.GetStreamAsync(uri, cancellationToken);

        internal Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            if (Headers is { Count: not 0 })
            {
                foreach (var header in Headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            return HttpClient.SendAsync(message, cancellationToken);
        }

        internal Task<HttpResponseMessage> ServerSentEventStreamAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            if (Headers is { Count: not 0 })
            {
                foreach (var header in Headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            return HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        /// <summary>
        /// Gets the full formatted url for the API endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint url.</param>
        /// <param name="queryParameters">Optional, parameters to add to the endpoint.</param>
        protected string GetUrl(string endpoint = "", Dictionary<string, string> queryParameters = null)
            => GetEndpoint(client.Settings.BaseRequestUrlFormat, endpoint, queryParameters);

        protected string GetWebsocketUri(string endpoint = "", Dictionary<string, string> queryParameters = null)
            => GetEndpoint(client.Settings.BaseWebSocketUrlFormat, endpoint, queryParameters);

        private string GetEndpoint(string baseUrlFormat, string endpoint = "", Dictionary<string, string> queryParameters = null)
        {
            string route;

            if (client.Settings.IsAzureOpenAI && IsAzureDeployment == true)
            {
                if (string.IsNullOrWhiteSpace(client.Settings.DeploymentId))
                {
                    throw new InvalidOperationException("Deployment ID must be provided for Azure OpenAI endpoints.");
                }

                route = $"deployments/{client.Settings.DeploymentId}/{Root}{endpoint}";
            }
            else
            {
                route = $"{Root}{endpoint}";
            }

            var result = string.Format(baseUrlFormat, route);

            foreach (var defaultQueryParameter in client.Settings.DefaultQueryParameters)
            {
                queryParameters ??= new Dictionary<string, string>();
                queryParameters.Add(defaultQueryParameter.Key, defaultQueryParameter.Value);
            }

            if (queryParameters is { Count: not 0 })
            {
                result += $"?{string.Join('&', queryParameters.Select(parameter => $"{parameter.Key}={parameter.Value}"))}";
            }

            return result;
        }
    }
}
