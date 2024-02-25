// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FunctionPropertyAttribute : Attribute
    {
        /// <summary>
        /// Property Attribute to help with function calling.
        /// </summary>
        /// <param name="description">
        /// The description of the property
        /// </param>
        /// <param name="required">
        /// Is the property required?
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="possibleValues">
        /// Enums or other possible values.
        /// </param>
        public FunctionPropertyAttribute(string description = null, bool required = false, object defaultValue = null, params object[] possibleValues)
        {
            Description = description;
            Required = required;
            DefaultValue = defaultValue;
            PossibleValues = possibleValues;
        }

        /// <summary>
        /// The description of the property
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Is the property required?
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// The default value.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Enums or other possible values.
        /// </summary>
        public object[] PossibleValues { get; }
    }
}
