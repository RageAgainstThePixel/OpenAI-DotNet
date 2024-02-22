// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class FunctionAttribute : Attribute
    {
        public FunctionAttribute(string description = null)
        {
            Description = description;
        }

        public string Description { get; }
    }
}