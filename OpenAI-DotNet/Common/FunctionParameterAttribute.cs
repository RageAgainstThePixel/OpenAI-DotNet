// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FunctionParameterAttribute : Attribute
    {
        public FunctionParameterAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}