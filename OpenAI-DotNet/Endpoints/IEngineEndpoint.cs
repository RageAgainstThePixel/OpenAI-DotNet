using OpenAI;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI.Endpoints
{

    public interface IEnginerEndpoint
    {
        /// <summary>
        /// The API endpoint for querying available Engines/models
        /// </summary>
        EnginesEndpoint EnginesEndpoint { get; }
    }
}
