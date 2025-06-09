// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class UserLocation
    {
        public UserLocation() { }

        public UserLocation(string city = null, string country = null, string region = null, string timezone = null)
        {
            City = city;
            Country = country;
            Region = region;
            Timezone = timezone;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "approximate";

        /// <summary>
        /// Free text input for the city of the user, e.g. San Francisco.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("city")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string City { get; private set; }

        /// <summary>
        /// The two-letter ISO country code of the user, e.g. US.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("country")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Country { get; private set; }

        /// <summary>
        /// Free text input for the region of the user, e.g. California.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("region")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Region { get; private set; }

        /// <summary>
        /// The IANA timezone of the user, e.g. America/Los_Angeles.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("timezone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Timezone { get; private set; }
    }
}
