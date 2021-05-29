namespace OpenAI_DotNet
{
    public abstract class BaseEndPoint
    {
        protected readonly OpenAI Api;

        /// <summary>
        /// Constructor of the api endpoint.
        /// Rather than instantiating this yourself, access it through an instance of <see cref="OpenAI"/>.
        /// </summary>
        internal BaseEndPoint(OpenAI api) => Api = api;

        /// <summary>
        /// Gets the basic endpoint url for the API
        /// </summary>
        /// <param name="engine">Optional, Engine to use for endpoint.</param>
        /// <returns>The completed basic url for the endpoint.</returns>
        protected abstract string GetEndpoint(Engine engine = null);
    }
}