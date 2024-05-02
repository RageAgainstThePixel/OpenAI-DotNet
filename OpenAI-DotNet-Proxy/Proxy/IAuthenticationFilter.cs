// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace OpenAI.Proxy
{
    /// <summary>
    /// Filters headers to ensure your users have the correct access.
    /// </summary>
    public interface IAuthenticationFilter
    {
        /// <summary>
        /// Checks the headers for your user issued token.
        /// If it's not valid, then throw <see cref="AuthenticationException"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="AuthenticationException"></exception>
        void ValidateAuthentication(IHeaderDictionary request);

        /// <summary>
        /// Checks the headers for your user issued token.
        /// If it's not valid, then throw <see cref="AuthenticationException"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="AuthenticationException"></exception>
        Task ValidateAuthenticationAsync(IHeaderDictionary request);
    }
}
