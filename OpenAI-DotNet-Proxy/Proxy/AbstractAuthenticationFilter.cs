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
