// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OpenAI.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Generates a <see cref="Guid"/> based on the string.
        /// </summary>
        /// <param name="string">The string to generate the <see cref="Guid"/>.</param>
        /// <returns>A new <see cref="Guid"/> that represents the string.</returns>
        public static Guid GenerateGuid(this string @string)
            => new(MD5.HashData(Encoding.UTF8.GetBytes(@string)));

        /// <summary>
        /// Attempts to get the event data from the string data.
        /// Returns false once the stream is done.
        /// </summary>
        /// <param name="streamData">Raw stream data.</param>
        /// <param name="eventData">Parsed stream data.</param>
        /// <returns>True, if the stream is not done. False if stream is done.</returns>
        public static bool TryGetEventStreamData(this string streamData, out string eventData)
        {
            const string dataTag = "data: ";
            eventData = string.Empty;

            if (streamData.StartsWith(dataTag))
            {
                eventData = streamData[dataTag.Length..].Trim();
            }

            const string doneTag = "[DONE]";
            return eventData != doneTag;
        }

        public static StringContent ToJsonStringContent(this string json)
        {
            const string jsonContent = "application/json";
            return new StringContent(json, null, jsonContent);
        }

        public static string ToSnakeCase(string @string)
            => string.IsNullOrEmpty(@string)
                ? @string
                : string.Concat(
                    @string.Select((x, i) => i > 0 && char.IsUpper(x)
                        ? $"_{x}"
                        : x.ToString())).ToLower();

        public static string ToEscapedJsonString<T>(this T @object)
            => JsonSerializer.Serialize(@object, escapedJsonOptions);

        private static readonly JsonSerializerOptions escapedJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
