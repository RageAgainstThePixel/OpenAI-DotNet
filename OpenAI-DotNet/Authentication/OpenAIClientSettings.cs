// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace OpenAI
{
    [Obsolete("use OpenAISettings instead")]
    public sealed class OpenAIClientSettings
    {
        public static implicit operator OpenAISettings(OpenAIClientSettings settings)
            => settings.Settings;

        public OpenAIClientSettings()
            => Settings = new OpenAISettings();

        public OpenAIClientSettings(string domain, string apiVersion = OpenAISettings.DefaultOpenAIApiVersion)
            => Settings = new OpenAISettings(domain, apiVersion);

        public OpenAIClientSettings(
            string resourceName,
            string deploymentId,
            string apiVersion = OpenAISettings.DefaultAzureApiVersion,
            bool useActiveDirectoryAuthentication = false,
            string azureDomain = OpenAISettings.AzureOpenAIDomain)
            => Settings = new OpenAISettings(resourceName, deploymentId, apiVersion, useActiveDirectoryAuthentication, azureDomain);

        private OpenAISettings Settings { get; }

        public string ResourceName => Settings.ResourceName;

        public string DeploymentId => Settings.DeploymentId;

        public string ApiVersion => Settings.ApiVersion;

        public string BaseRequest => Settings.BaseRequest;

        internal string BaseRequestUrlFormat => Settings.BaseRequestUrlFormat;

        internal string BaseWebSocketUrlFormat => Settings.BaseWebSocketUrlFormat;

        internal bool UseOAuthAuthentication => Settings.UseOAuthAuthentication;

        public bool IsAzureOpenAI => UseOAuthAuthentication;

        internal IReadOnlyDictionary<string, string> DefaultQueryParameters => Settings.DefaultQueryParameters;

        public static OpenAIClientSettings Default { get; } = new();
    }
}
