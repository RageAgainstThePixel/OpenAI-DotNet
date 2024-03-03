// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FunctionParameterAttribute : Attribute
    {
        /// <summary>
        /// Function parameter attribute to help describe the parameter for the function.
        /// </summary>
        /// <param name="description">The description of the parameter and its usage.</param>
        public FunctionParameterAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}