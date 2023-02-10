using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class AuthInfo
    {
        [JsonConstructor]
        public AuthInfo(string apiKey, string organizationId = null)
        {
            if (!apiKey.Contains("sk-"))
            {
                throw new InvalidCredentialException($"{apiKey} parameter must start with 'sk-'");
            }

            ApiKey = apiKey;

            if (organizationId != null)
            {
                if (!organizationId.Contains("org-"))
                {
                    throw new InvalidCredentialException($"{nameof(organizationId)} parameter must start with 'org-'");
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
