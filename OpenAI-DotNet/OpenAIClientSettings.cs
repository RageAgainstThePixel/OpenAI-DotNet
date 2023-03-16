using System;

namespace OpenAI
{
    /// <summary>
    /// The client settings for configuring Azure OpenAI or custom domain.
    /// </summary>
    public sealed class OpenAIClientSettings
    {
        internal const string OpenAIDomain = "api.openai.com";
        internal const string AzureOpenAIDomain = "openai.azure.com";

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        public OpenAIClientSettings()
        {
            ResourceName = OpenAIDomain;
            ApiVersion = "v1";
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"https://{ResourceName}{BaseRequest}{{0}}";
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        /// <param name="domain">Base api domain.</param>
        /// <param name="apiVersion">The version of the OpenAI api you want to use.</param>
        public OpenAIClientSettings(string domain, string apiVersion = "v1")
        {
            if (!domain.Contains(".") &&
                !domain.Contains(":"))
            {
                throw new ArgumentException($"You're attempting to pass a \"resourceName\" parameter to \"{nameof(domain)}\". Please specify \"resourceName:\" for this parameter in constructor.");
            }

            ResourceName = domain;
            ApiVersion = apiVersion;
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"https://{ResourceName}{BaseRequest}{{0}}";
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
        public OpenAIClientSettings(string resourceName, string deploymentId, string apiVersion = "2022-12-01")
        {
            if (resourceName.Contains(".") ||
                resourceName.Contains(":"))
            {
                throw new ArgumentException($"You're attempting to pass a \"domain\" parameter to \"{nameof(resourceName)}\". Please specify \"domain:\" for this parameter in constructor.");
            }

            ResourceName = resourceName;
            DeploymentId = deploymentId;
            ApiVersion = apiVersion;
            BaseRequest = $"/openai/deployments/{DeploymentId}/";
            BaseRequestUrlFormat = $"https://{ResourceName}.{AzureOpenAIDomain}{BaseRequest}{{0}}?api-version={ApiVersion}";
        }

        public string ResourceName { get; }

        public string ApiVersion { get; }

        public string DeploymentId { get; }

        internal string BaseRequest { get; }

        internal string BaseRequestUrlFormat { get; }

        public static OpenAIClientSettings Default { get; } = new OpenAIClientSettings();
    }
}
