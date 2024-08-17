// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OpenAI
{
    public abstract class OpenAIBaseEndpoint
    {
        protected OpenAIBaseEndpoint(OpenAIClient client) => this.client = client;

        // ReSharper disable once InconsistentNaming
        protected readonly OpenAIClient client;

        internal HttpClient HttpClient => client.Client;

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
        protected abstract bool? IsAzureDeployment { get; }

        /// <summary>
        /// Gets the full formatted url for the API endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint url.</param>
        /// <param name="queryParameters">Optional, parameters to add to the endpoint.</param>
        protected string GetUrl(string endpoint = "", Dictionary<string, string> queryParameters = null)
        {
            string result;

            if (client.OpenAIClientSettings.IsAzureOpenAI)
            {
                var format = IsAzureDeployment == true
                    ? client.OpenAIClientSettings.DeploymentUrlFormat
                    : client.OpenAIClientSettings.BaseRequestUrlFormat;
                result = string.Format(format, $"{Root}{endpoint}");
            }
            else
            {
                result = string.Format(client.OpenAIClientSettings.BaseRequestUrlFormat, $"{Root}{endpoint}");
            }

            foreach (var defaultQueryParameter in client.OpenAIClientSettings.DefaultQueryParameters)
            {
                queryParameters ??= new Dictionary<string, string>();
                queryParameters.Add(defaultQueryParameter.Key, defaultQueryParameter.Value);
            }

            if (queryParameters is { Count: not 0 })
            {
                result += $"?{string.Join("&", queryParameters.Select(parameter => $"{parameter.Key}={parameter.Value}"))}";
            }

            return result;
        }

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
    }
}
