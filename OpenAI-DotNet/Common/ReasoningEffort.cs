// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI
{
    /// <summary>
    /// Constrains the effort of reasoning for <see href="https://platform.openai.com/docs/guides/reasoning">Reasoning Models</see>.<br/>
    /// Currently supported values are: Low, Medium, High. Reducing reasoning effort can result in faster responses and fewer tokens used on reasoning response.
    /// </summary>
    /// <remarks>
    /// <b>o1 models only!</b>
    /// </remarks>
    public enum ReasoningEffort
    {
        Low = 1,
        Medium,
        High
    }
}
