using System.Security.Authentication;
using Microsoft.AspNetCore.Http;

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
    }
}
