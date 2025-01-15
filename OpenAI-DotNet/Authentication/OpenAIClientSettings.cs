// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace OpenAI
{
    /// <summary>
    /// The client settings for configuring Azure OpenAI or custom domain.
    /// </summary>
    public sealed class OpenAIClientSettings
    {
        internal const string WS = "ws://";
        internal const string WSS = "wss://";
        internal const string Http = "http://";
        internal const string Https = "https://";
        internal const string OpenAIDomain = "api.openai.com";
        internal const string DefaultOpenAIApiVersion = "v1";
        internal const string AzureOpenAIDomain = "openai.azure.com";
        internal const string DefaultAzureApiVersion = "2023-05-01";

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        public OpenAIClientSettings()
        {
            ResourceName = OpenAIDomain;
            ApiVersion = DefaultOpenAIApiVersion;
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"{Https}{ResourceName}{BaseRequest}{{0}}";
            BaseWebSocketUrlFormat = $"{WSS}{ResourceName}{BaseRequest}{{0}}";
            UseOAuthAuthentication = true;
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        /// <param name="domain">Base api domain.</param>
        /// <param name="apiVersion">The version of the OpenAI api you want to use.</param>
        public OpenAIClientSettings(string domain, string apiVersion = DefaultOpenAIApiVersion)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = OpenAIDomain;
            }

            if (!domain.Contains('.') &&
                !domain.Contains(':'))
            {
                throw new ArgumentException($"You're attempting to pass a \"resourceName\" parameter to \"{nameof(domain)}\". Please specify \"resourceName:\" for this parameter in constructor.");
            }

            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                apiVersion = DefaultOpenAIApiVersion;
            }

            var protocol = Https;

            if (domain.StartsWith(Http))
            {
                protocol = Http;
                domain = domain.Replace(Http, string.Empty);
            }
            else if (domain.StartsWith(Https))
            {
                protocol = Https;
                domain = domain.Replace(Https, string.Empty);
            }

            ResourceName = $"{protocol}{domain}";
            ApiVersion = apiVersion;
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"{ResourceName}{BaseRequest}{{0}}";
            BaseWebSocketUrlFormat = ResourceName.Contains(Https)
                ? $"{WSS}{domain}{BaseRequest}{{0}}"
                : $"{WS}{domain}{BaseRequest}{{0}}";
            UseOAuthAuthentication = true;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenAIClientSettings"/> for use with Azure OpenAI.<br/>
        /// <see href="https://learn.microsoft.com/en-us/azure/cognitive-services/openai/"/>
        /// </summary>
        /// <param name="resourceName">
        /// The name of your Azure OpenAI Resource.
        /// </param>
        /// <param name="deploymentId">
        /// The name of your model deployment. You're required to first deploy a model before you can make calls.
        /// </param>
        /// <param name="apiVersion">
        /// Optional, defaults to 2022-12-01
        /// </param>
        /// <param name="useActiveDirectoryAuthentication">
        /// Optional, set to true if you want to use Azure Active Directory for Authentication.
        /// </param>
        public OpenAIClientSettings(string resourceName, string deploymentId, string apiVersion = DefaultAzureApiVersion, bool useActiveDirectoryAuthentication = false)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            if (resourceName.Contains('.') ||
                resourceName.Contains(':'))
            {
                throw new ArgumentException($"You're attempting to pass a \"domain\" parameter to \"{nameof(resourceName)}\". Please specify \"domain:\" for this parameter in constructor.");
            }

            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                apiVersion = DefaultAzureApiVersion;
            }

            ResourceName = resourceName;
            DeploymentId = deploymentId;
            ApiVersion = apiVersion;
            BaseRequest = "/openai/";
            BaseRequestUrlFormat = $"{Https}{ResourceName}.{AzureOpenAIDomain}{BaseRequest}{{0}}";
            BaseWebSocketUrlFormat = $"{WSS}{ResourceName}.{AzureOpenAIDomain}{BaseRequest}{{0}}";
            defaultQueryParameters.Add("api-version", ApiVersion);
            UseOAuthAuthentication = useActiveDirectoryAuthentication;
        }

        public string ResourceName { get; }

        public string DeploymentId { get; }

        public string ApiVersion { get; }

        public string BaseRequest { get; }

        internal string BaseRequestUrlFormat { get; }

        internal string BaseWebSocketUrlFormat { get; }

        internal bool UseOAuthAuthentication { get; }

        public bool IsAzureOpenAI => BaseRequestUrlFormat.Contains(AzureOpenAIDomain);

        private readonly Dictionary<string, string> defaultQueryParameters = new();

        internal IReadOnlyDictionary<string, string> DefaultQueryParameters => defaultQueryParameters;

        public static OpenAIClientSettings Default { get; } = new();
    }
}
