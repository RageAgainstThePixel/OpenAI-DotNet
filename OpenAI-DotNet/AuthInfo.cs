using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class AuthInfo
    {
        private const string SecretKeyPrefix = "sk-";
        private const string SessionKeyPrefix = "sess-";
        private const string OrganizationPrefix = "org-";

        [JsonConstructor]
        public AuthInfo(string apiKey, string organizationId = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey) ||
                (!apiKey.Contains(SecretKeyPrefix) &&
                 !apiKey.Contains(SessionKeyPrefix)))
            {
                throw new InvalidCredentialException($"{apiKey} must start with '{SecretKeyPrefix}'");
            }

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
