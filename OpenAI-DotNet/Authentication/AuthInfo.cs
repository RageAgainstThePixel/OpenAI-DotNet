// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class AuthInfo
    {
        internal const string SecretKeyPrefix = "sk-";
        internal const string SessionKeyPrefix = "sess-";
        internal const string OrganizationPrefix = "org-";

        [JsonConstructor]
        public AuthInfo(string apiKey, string organizationId = null)
        {
            ApiKey = apiKey;

            if (!string.IsNullOrWhiteSpace(organizationId))
            {
                if (!organizationId.Contains(OrganizationPrefix))
                {
                    throw new InvalidCredentialException($"{nameof(organizationId)} must start with '{OrganizationPrefix}'");
                }

                OrganizationId = organizationId;
            }
        }

        [JsonPropertyName("apiKey")]
        public string ApiKey { get; }

        [JsonPropertyName("organization")]
        public string OrganizationId { get; }
    }
}
