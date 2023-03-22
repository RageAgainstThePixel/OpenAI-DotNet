using System;
using System.Globalization;

namespace OpenAI
{
    public abstract class BaseEndPoint
    {
        protected BaseEndPoint(OpenAIClient api)
        {
            this.Api = api;
        }

        protected OpenAIClient Api { get; init; }

        /// <summary>
        /// The root endpoint address.
        /// </summary>
        protected abstract string Root { get; }

        /// <summary>
        /// Gets the full formatted url for the API endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint url.</param>
        protected string GetUrl(string endpoint = "")
        {
            return String.Format(CultureInfo.CurrentCulture, this.Api.OpenAIClientSettings.BaseRequestUrlFormat, $"{this.Root}{endpoint}");
        }
    }
}
