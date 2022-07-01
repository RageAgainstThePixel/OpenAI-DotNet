using OpenAI;

namespace OpenAI.Endpoints
{
    public interface ICompletionEndpoint
    {
        /// <summary>
        /// Text generation is the core function of the API.
        /// You give the API a prompt, and it generates a completion.
        /// The way you “program” the API to do a task is by simply describing the task in plain english
        /// or providing a few written examples. This simple approach works for a wide range of use cases,
        /// including summarization, translation, grammar correction, question answering, chatbots, composing emails,
        /// and much more (see the prompt library for inspiration).
        /// </summary>
        CompletionEndpoint CompletionEndpoint { get; }
    }
}
