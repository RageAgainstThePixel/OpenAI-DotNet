namespace OpenAI
{
    public abstract class BaseEndPoint
    {
        protected BaseEndPoint(OpenAIClient api) => Api = api;

        protected readonly OpenAIClient Api;

        /// <summary>
        /// The root endpoint address.
        /// </summary>
        protected abstract string Root { get; }

        /// <summary>
        /// Gets the full formatted url for the API endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint url.</param>
        protected string GetUrl(string endpoint = "")
            => string.Format(Api.OpenAIClientSettings.BaseRequestUrlFormat, $"{Root}{endpoint}");
    }
}
