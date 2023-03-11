namespace OpenAI
{
    public sealed class OpenAIClientSettings
    {
        internal const string OpenAIDomain = "api.openai.com";

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        public OpenAIClientSettings()
        {
            ResourceName = OpenAIDomain;
            ApiVersion = "1";
            DeploymentId = string.Empty;
            BaseRequestUrl = $"https://{OpenAIDomain}/v{ApiVersion}/{{0}}";
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenAIClientSettings"/> for use with OpenAI.
        /// </summary>
        /// <param name="apiVersion">The version of the OpenAI api you want to use.</param>
        public OpenAIClientSettings(string apiVersion)
        {
            ResourceName = OpenAIDomain;

            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                apiVersion = "1";
            }

            ApiVersion = apiVersion;
            DeploymentId = string.Empty;
            BaseRequestUrl = $"https://{OpenAIDomain}/v{ApiVersion}/{{0}}";
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
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                resourceName = OpenAIDomain;
            }

            ResourceName = resourceName;
            DeploymentId = deploymentId;
            ApiVersion = apiVersion;
            BaseRequestUrl = $"https://{ResourceName}.openai.azure.com/openai/deployments/{DeploymentId}/{{0}}?api-version={ApiVersion}";
        }

        public string ResourceName { get; }

        public string ApiVersion { get; }

        public string DeploymentId { get; }

        internal string BaseRequestUrl { get; }

        public static OpenAIClientSettings Default { get; } = new OpenAIClientSettings();
    }
}