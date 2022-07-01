using System;

namespace OpenAI
{
    public abstract class BaseEndPoint
    {
        protected readonly BaseOpenAIClient Api;

        /// <summary>
        /// Delegate method for creating Url's dynamically for a given engine deployment type.
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        public delegate string UrlGenerator(Engine deployment);

        /// <summary>
        /// The Url Generator for each endpoint, changes based on whether the service is OAI or AOAI
        /// </summary>
        protected readonly UrlGenerator Generator;

        /// <summary>
        /// Constructor of the api endpoint.
        /// Rather than instantiating this yourself, access it through an instance of <see cref="OpenAIClient"/>.
        /// </summary>
        internal BaseEndPoint(BaseOpenAIClient api, UrlGenerator generator)
        {
            Api = api;
            Generator = generator;

        } 

        /// <summary>
        /// Gets the basic endpoint url for the API
        /// </summary>
        /// <param name="engine">Optional, Engine to use for endpoint.</param>
        /// <returns>The completed basic url for the endpoint.</returns>
        protected string GetEndpoint(Engine engine = null)
        {
            return Generator(engine ?? Api.DefaultEngine);
        }


    }
}