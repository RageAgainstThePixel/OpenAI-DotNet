// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace OpenAI.Proxy
{
    /// <inheritdoc />
    public abstract class AbstractAuthenticationFilter : IAuthenticationFilter
    {
        /// <inheritdoc />
        public abstract void ValidateAuthentication(IHeaderDictionary request);
    }
}
