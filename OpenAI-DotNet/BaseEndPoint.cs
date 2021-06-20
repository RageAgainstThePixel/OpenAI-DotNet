namespace OpenAI
{
    public abstract class BaseEndPoint
    {
        protected readonly OpenAIClient Api;

        /// <summary>
        /// Constructor of the api endpoint.
        /// Rather than instantiating this yourself, access it through an instance of <see cref="OpenAIClient"/>.
        /// </summary>
        internal BaseEndPoint(OpenAIClient api) => Api = api;

        /// <summary>
        /// Gets the basic endpoint url for the API
        /// </summary>
        /// <param name="engine">Optional, Engine to use for endpoint.</param>
        /// <returns>The completed basic url for the endpoint.</returns>
        protected abstract string GetEndpoint(Engine engine = null);
    }
}