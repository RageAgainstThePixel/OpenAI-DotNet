// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace OpenAI.Tests.Weather
{
    internal static class DateTimeUtility
    {
        [Function("Get the current date and time.")]
        public static async Task<string> GetDateTime()
            => await Task.FromResult(DateTimeOffset.Now.ToString());
    }
}
