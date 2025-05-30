// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace OpenAI
{
    public enum TextResponseFormat
    {
        Auto = 0,
        /// <summary>
        /// Default response format. Used to generate text responses.
        /// </summary>
        [EnumMember(Value = "text")]
        Text,
        /// <summary>
        /// JSON object response format.
        /// An older method of generating JSON responses.
        /// Using `json_schema` is recommended for models that support it.
        /// Note that the model will not generate JSON without a system or user message
        /// instructing it to do so.
        /// </summary>
        /// <remarks>
        /// Not recommended for gpt-4o and newer models!
        /// </remarks>
        [Obsolete("use JsonSchema instead")]
        [EnumMember(Value = "json_object")]
        Json,
        /// <summary>
        /// JSON Schema response format. Used to generate structured JSON responses.
        /// </summary>
        [EnumMember(Value = "json_schema")]
        JsonSchema
    }
}
