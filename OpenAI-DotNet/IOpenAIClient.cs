using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Completions;
using OpenAI.Edits;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;

namespace OpenAI;

public interface IOpenAIClient
{
    /// <summary>
    /// The API authentication information to use for API calls
    /// </summary>
    OpenAIAuthentication OpenAIAuthentication { get; }
    
    /// <summary>
    /// List and describe the various models available in the API.
    /// You can refer to the Models documentation to understand what <see href="https://platform.openai.com/docs/models"/> are available and the differences between them.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/models"/>
    /// </summary>
    IModelsEndpoint ModelsEndpoint { get; }
    
    /// <summary>
    /// Text generation is the core function of the API. You give the API a prompt, and it generates a completion.
    /// The way you “program” the API to do a task is by simply describing the task in plain english or providing
    /// a few written examples. This simple approach works for a wide range of use cases, including summarization,
    /// translation, grammar correction, question answering, chatbots, composing emails, and much more
    /// (see the prompt library for inspiration).<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/completions"/>
    /// </summary>
    ICompletionsEndpoint CompletionsEndpoint { get; }
    
    /// <summary>
    /// Given a chat conversation, the model will return a chat completion response.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/chat"/>
    /// </summary>
    IChatEndpoint ChatEndpoint { get; }
    
    /// <summary>
    /// Given a prompt and an instruction, the model will return an edited version of the prompt.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/edits"/>
    /// </summary>
    IEditsEndpoint EditsEndpoint { get; }
    
    /// <summary>
    /// Given a prompt and/or an input image, the model will generate a new image.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/images"/>
    /// </summary>
    IImagesEndpoint ImagesEndPoint { get; }
    
    /// <summary>
    /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.<br/>
    /// <see href="https://platform.openai.com/docs/guides/embeddings"/>
    /// </summary>
    IEmbeddingsEndpoint EmbeddingsEndpoint { get; }
    
    /// <summary>
    /// Transforms audio into text.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/audio"/>
    /// </summary>
    IAudioEndpoint AudioEndpoint { get; }
    
    /// <summary>
    /// Files are used to upload documents that can be used with features like Fine-tuning.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/fine-tunes"/>
    /// </summary>
    IFilesEndpoint FilesEndpoint { get; }
    
    /// <summary>
    /// Manage fine-tuning jobs to tailor a model to your specific training data.<br/>
    /// <see href="https://platform.openai.com/docs/guides/fine-tuning"/>
    /// </summary>
    IFineTuningEndpoint FineTuningEndpoint { get; }
    
    /// <summary>
    /// The moderation endpoint is a tool you can use to check whether content complies with OpenAI's content policy.
    /// Developers can thus identify content that our content policy prohibits and take action, for instance by filtering it.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/moderations"/>
    /// </summary>
    IModerationsEndpoint ModerationsEndpoint { get; }
}