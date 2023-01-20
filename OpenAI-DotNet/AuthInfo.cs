using System;
using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace OpenAI
{
    [Serializable]
    internal class AuthInfo
    {
        public AuthInfo(string apiKey, string organization = null)
        {
            if (!apiKey.Contains("sk-"))
            {
                throw new InvalidCredentialException($"{apiKey} parameter must start with 'sk-'");
            }

            ApiKey = apiKey;

            if (organization != null)
            {
                if (!organization.Contains("org-"))
                {
                    throw new InvalidCredentialException($"{nameof(organization)} parameter must start with 'org-'");
                }

                Organization = organization;
            }
        }

        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }
    }
}