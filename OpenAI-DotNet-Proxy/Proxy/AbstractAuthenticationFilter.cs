// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace OpenAI.Proxy
{
    /// <inheritdoc />
    public abstract class AbstractAuthenticationFilter : IAuthenticationFilter
    {
        [Obsolete("Use ValidateAuthenticationAsync")]
        public virtual void ValidateAuthentication(IHeaderDictionary request) { }

        /// <inheritdoc />
        public abstract Task ValidateAuthenticationAsync(IHeaderDictionary request);
    }
}
