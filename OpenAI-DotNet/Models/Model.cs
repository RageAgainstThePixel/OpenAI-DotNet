// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Models
{
    /// <summary>
    /// Represents a language model.<br/>
    /// <see href="https://platform.openai.com/docs/models/model-endpoint-compatability"/>
    /// </summary>
    public sealed class Model
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Model id.</param>
        /// <param name="ownedBy">Optional, owned by id.</param>
        public Model(string id, string ownedBy = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id), "Missing the id of the specified model.");
            }

            Id = id;
            OwnedBy = ownedBy;
        }

        /// <summary>
        /// Allows a model to be implicitly cast to the string of its id.
        /// </summary>
        /// <param name="model">The <see cref="Model"/> to cast to a string.</param>
        public static implicit operator string(Model model) => model?.ToString();

        /// <summary>
        /// Allows a string to be implicitly cast as a <see cref="Model"/>
        /// </summary>
        public static implicit operator Model(string name) => new(name);

        /// <inheritdoc />
        public override string ToString() => Id;

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; private set; }

        [JsonInclude]
        [JsonPropertyName("permission")]
        public IReadOnlyList<Permission> Permissions { get; private set; }

        [JsonInclude]
        [JsonPropertyName("root")]
        public string Root { get; private set; }

        [JsonInclude]
        [JsonPropertyName("parent")]
        public string Parent { get; private set; }

        #region Reasoning Models

        /// <summary>
        /// GPT-5.2 pro is available in the Responses API only to enable support for multi-turn model interactions before responding to API requests,
        /// and other advanced API features in the future. Since GPT-5.2 pro is designed to tackle tough problems,
        /// some requests may take several minutes to finish. To avoid timeouts, try using background mode.
        /// GPT-5.2 pro supports reasoning.effort: medium, high, xhigh.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 context window<br/>
        /// - Max Output Tokens: 128,000 max output tokens
        /// </remarks>
        public static Model GPT5_2_Pro { get; } = new("gpt-5.2-pro", "openai");

        /// <summary>
        /// The o1 series of models are trained with reinforcement learning to perform complex reasoning.
        /// o1 models think before they answer, producing a long internal chain of thought before responding to the user.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O1 { get; } = new("o1", "openai");

        /// <summary>
        /// The o1 reasoning model is designed to solve hard problems across domains.
        /// o1-mini is a faster and more affordable reasoning model,
        /// but we recommend using the newer o3-mini model that features higher intelligence at the same latency and price as o1-mini.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 65,536 max output tokens
        /// </remarks>
        [Obsolete("Deprecated")]
        public static Model O1Mini { get; } = new("o1-mini", "openai");

        /// <summary>
        /// The o1 series of models are trained with reinforcement learning to think before they answer and perform complex reasoning.
        /// The o1-pro model uses more compute to think harder and provide consistently better answers.
        /// o1-pro is available in the Responses API only to enable support for multi-turn model interactions before responding to API requests,
        /// and other advanced API features in the future.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O1Pro { get; } = new("o1-pro", "openai");

        /// <summary>
        /// o3 is a well-rounded and powerful model across domains.
        /// It sets a new standard for math, science, coding, and visual reasoning tasks.
        /// It also excels at technical writing and instruction-following.
        /// Use it to think through multi-step problems that involve analysis across text, code, and images.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O3 { get; } = new("o3", "openai");

        /// <summary>
        /// The o-series of models are trained with reinforcement learning to think before they answer and perform complex reasoning.
        /// The o3-pro model uses more compute to think harder and provide consistently better answers.<br/>
        /// o3-pro is available in the Responses API only to enable support for multi-turn model interactions before responding to API requests,
        /// and other advanced API features in the future. Since o3-pro is designed to tackle tough problems, some requests may take several minutes to finish.
        /// To avoid timeouts, try using background mode.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O3Pro { get; } = new("o3-pro", "openai");

        /// <summary>
        /// o3-mini is our newest small reasoning model, providing high intelligence at the same cost and latency targets of o1-mini.
        /// o3-mini supports key developer features, like Structured Outputs, function calling, and Batch API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O3Mini { get; } = new("o3-mini", "openai");

        /// <summary>
        /// o4-mini is our latest small o-series model.
        /// It's optimized for fast, effective reasoning with exceptionally efficient performance in coding and visual tasks.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model O4Mini { get; } = new("o4-mini", "openai");

        #endregion Reasoning Models

        #region Realtime Models

        /// <summary>
        /// This is our first general-availability realtime model, capable of responding to audio and text inputs in realtime over WebRTC, WebSocket, or SIP connections.
        /// </summary>
        /// <remarks>
        /// - Context Window: 32,000 tokens<br/>
        /// - Max Output Tokens: 4,096 tokens
        /// </remarks>
        public static Model GPT_Realtime { get; } = new("gpt-realtime", "openai");

        /// <summary>
        /// This is a preview release of the GPT-4o Realtime model, capable of responding to audio and text inputs in realtime over WebRTC or a WebSocket interface.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 4,096 tokens
        /// </remarks>
        public static Model GPT4oRealtime { get; } = new("gpt-4o-realtime-preview", "openai");

        /// <summary>
        /// This is a preview release of the GPT-4o-mini Realtime model, capable of responding to audio and text inputs in realtime over WebRTC or a WebSocket interface.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 4,096 tokens
        /// </remarks>
        public static Model GPT4oRealtimeMini { get; } = new("gpt-4o-mini-realtime-preview", "openai");

        #endregion Realtime Models

        #region Chat Models

        /// <summary>
        /// GPT-5.2 is our flagship model for coding and agentic tasks across industries. 
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 context window<br/>
        /// - Max Output Tokens: 128,000 max output tokens
        /// </remarks>
        public static Model GPT5_2 { get; } = new("gpt-5.2", "openai");

        /// <summary>
        /// GPT-5 is our flagship model for coding, reasoning, and agentic tasks across domains.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 context window<br/>
        /// - Max Output Tokens: 128,000 max output tokens
        /// </remarks>
        public static Model GPT5 { get; } = new("gpt-5", "openai");

        /// <summary>
        /// GPT-5 mini is a faster, more cost-efficient version of GPT-5. It's great for well-defined tasks and precise prompts.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 context window<br/>
        /// - Max Output Tokens: 128,000 max output tokens
        /// </remarks>
        public static Model GPT5_Mini { get; } = new("gpt-5-mini", "openai");

        /// <summary>
        /// GPT-5 Nano is our fastest, cheapest version of GPT-5. It's great for summarization and classification tasks.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 context window<br/>
        /// - Max Output Tokens: 128,000 max output tokens
        /// </remarks>
        public static Model GPT5_Nano { get; } = new("gpt-5-nano", "openai");

        /// <summary>
        /// GPT-5 Chat points to the GPT-5 snapshot currently used in ChatGPT.
        /// We recommend GPT-5 for most API usage,
        /// but feel free to use this GPT-5 Chat model to test our latest improvements for chat use cases.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 context window<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        public static Model GPT5_Chat { get; } = new("gpt-5-chat-latest", "openai");

        /// <summary>
        /// ChatGPT-4o points to the GPT-4o snapshot currently used in ChatGPT.
        /// GPT-4o is our versatile, high-intelligence flagship model.
        /// It accepts both text and image inputs, and produces text outputs.
        /// It is the best model for most tasks, and is our most capable model outside of our o-series models.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 tokens
        /// </remarks>
        public static Model ChatGPT4o { get; } = new("chatgpt-4o-latest", "openai");

        /// <summary>
        /// GPT-4o (“o” for “omni”) is our versatile, high-intelligence flagship model.
        /// It accepts both text and image inputs, and produces text outputs (including Structured Outputs).
        /// It is the best model for most tasks, and is our most capable model outside of our o-series models.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 tokens
        /// </remarks>
        public static Model GPT4o { get; } = new("gpt-4o", "openai");

        /// <summary>
        /// GPT-4o mini (“o” for “omni”) is a fast, affordable small model for focused tasks.
        /// It accepts both text and image inputs, and produces text outputs (including Structured Outputs).
        /// It is ideal for fine-tuning, and model outputs from a larger model like GPT-4o can be distilled
        /// to GPT-4o-mini to produce similar results at lower cost and latency.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        public static Model GPT4oMini { get; } = new("gpt-4o-mini", "openai");

        /// <summary>
        /// This is a preview release of the GPT-4o Audio models.
        /// These models accept audio inputs and outputs, and can be used in the Chat Completions REST API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        public static Model GPT4oAudio { get; } = new("gpt-4o-audio-preview", "openai");

        /// <summary>
        /// This is a preview release of the smaller GPT-4o Audio mini model. It's designed to input audio or create audio outputs via the REST API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        public static Model GPT4oAudioMini { get; } = new("gpt-4o-mini-audio-preview", "openai");

        /// <summary>
        /// GPT-4.1 is our flagship model for complex tasks. It is well suited for problem solving across domains.
        /// </summary>
        /// <remarks>
        /// - Context Window: 1,047,576 context window<br/>
        /// - Max Output Tokens: 32,768 max output tokens
        /// </remarks>
        public static Model GPT4_1 { get; } = new("gpt-4.1", "openai");

        /// <summary>
        /// GPT-4.1 mini provides a balance between intelligence, speed, and cost that makes it an attractive model for many use cases.
        /// </summary>
        /// <remarks>
        /// - Context Window: 1,047,576 context window<br/>
        /// - Max Output Tokens: 32,768 max output tokens
        /// </remarks>
        public static Model GPT4_1_Mini { get; } = new("gpt-4.1-mini", "openai");

        /// <summary>
        /// GPT-4.1 nano is the fastest, most cost-effective GPT-4.1 model.
        /// </summary>
        /// <remarks>
        /// - Context Window: 1,047,576 context window<br/>
        /// - Max Output Tokens: 32,768 max output tokens
        /// </remarks>
        public static Model GPT4_1_Nano { get; } = new("gpt-4.1-nano", "openai");

        /// <summary>
        /// Deprecated - a research preview of GPT-4.5. We recommend using gpt-4.1 or o3 models instead for most use cases.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 context window<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        [Obsolete("Deprecated")]
        public static Model GPT4_5 { get; } = new("gpt-4.5-preview", "openai");

        /// <summary>
        /// GPT-4 is an older version of a high-intelligence GPT model, usable in Chat Completions.
        /// </summary>
        /// <remarks>
        /// - Context Window: 8,192 tokens<br/>
        /// - Max Output Tokens: 8,192 max output tokens
        /// </remarks>
        public static Model GPT4 { get; } = new("gpt-4", "openai");

        /// <summary>
        /// GPT-4 Turbo is the next generation of GPT-4, an older high-intelligence GPT model.
        /// It was designed to be a cheaper, better version of GPT-4. Today, we recommend using a newer model like GPT-4o.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 4,096 max output tokens
        /// </remarks>
        public static Model GPT4_Turbo { get; } = new("gpt-4-turbo", "openai");

        /// <summary>
        /// Same capabilities as the base gpt-4 mode but with 4x the context length.
        /// Will be updated with our latest model iteration.  Tokens are 2x the price of gpt-4.
        /// </summary>
        public static Model GPT4_32K { get; } = new("gpt-4-32k", "openai");

        /// <summary>
        /// GPT-3.5 Turbo models can understand and generate natural language or code and have been optimized
        /// for chat using the Chat Completions API but work well for non-chat tasks as well. As of July 2024,
        /// use gpt-4o-mini in place of GPT-3.5 Turbo, as it is cheaper, more capable, multimodal, and just as fast.
        /// GPT-3.5 Turbo is still available for use in the API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 16,385 tokens<br/>
        /// - Max Output Tokens: 4,096 max output tokens
        /// </remarks>
        public static Model GPT3_5_Turbo { get; } = new("gpt-3.5-turbo", "openai");

        /// <summary>
        /// Same capabilities as the base gpt-3.5-turbo mode but with 4x the context length.
        /// Tokens are 2x the price of gpt-3.5-turbo. Will be updated with our latest model iteration.
        /// </summary>
        public static Model GPT3_5_Turbo_16K { get; } = new("gpt-3.5-turbo-16k", "openai");

        #endregion Chat Models

        #region GPT Base Models

        /// <summary>
        /// Replacement for the GPT-3 curie and davinci base models.
        /// </summary>
        public static Model Davinci { get; } = new("davinci-002", "openai");

        /// <summary>
        /// Replacement for the GPT-3 ada and babbage base models.
        /// </summary>
        public static Model Babbage { get; } = new("babbage-002", "openai");

        #endregion GPT Base Models

        #region Embedding Models

        /// <summary>
        /// Most capable embedding model for both english and non-english tasks.
        /// </summary>
        /// <remarks>
        /// Output Dimension: 3,072
        /// </remarks>
        public static Model Embedding_3_Large { get; } = new("text-embedding-3-large", "openai");

        /// <summary>
        /// A highly efficient model which provides a significant upgrade over its predecessor, the text-embedding-ada-002 model.
        /// </summary>
        /// <remarks>
        /// Output Dimension: 1,536
        /// </remarks>
        public static Model Embedding_3_Small { get; } = new("text-embedding-3-small", "openai");

        /// <summary>
        /// The default model for <see cref="Embeddings.EmbeddingsEndpoint"/>.
        /// </summary>
        /// <remarks>
        /// Output Dimension: 1,536
        /// </remarks>
        public static Model Embedding_Ada_002 { get; } = new("text-embedding-ada-002", "openai");

        #endregion Embedding Models

        #region Moderation Models

        public static Model OmniModerationLatest { get; } = new("omni-moderation-latest", "openai");

        [Obsolete("use OmniModerationLatest")]
        public static Model Moderation_Latest { get; } = new("text-moderation-latest", "openai");

        [Obsolete("use OmniModerationLatest")]
        public static Model Moderation_Stable { get; } = new("text-moderation-stable", "openai");

        #endregion Moderation Models

        #region Audio Models

        /// <summary>
        /// The gpt-audio model is our first generally available audio model.
        /// It accepts audio inputs and outputs, and can be used in the Chat Completions REST API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 128,000 tokens<br/>
        /// - Max Output Tokens: 16,384 max output tokens
        /// </remarks>
        public static Model GPT_Audio { get; } = new("gpt-audio", "openai");

        /// <summary>
        /// TTS is a model that converts text to natural sounding spoken text.
        /// The tts-1-hd model is optimized for high quality text-to-speech use cases.
        /// Use it with the Speech endpoint in the Audio API.
        /// </summary>
        public static Model TTS_1 { get; } = new("tts-1", "openai");

        /// <summary>
        /// TTS is a model that converts text to natural sounding spoken text.
        /// The tts-1-hd model is optimized for high quality text-to-speech use cases.
        /// Use it with the Speech endpoint in the Audio API.
        /// </summary>
        public static Model TTS_1HD { get; } = new("tts-1-hd", "openai");

        /// <summary>
        /// GPT-4o mini TTS is a text-to-speech model built on GPT-4o mini, a fast and powerful language model.
        /// Use it to convert text to natural sounding spoken text.
        /// </summary>
        /// <remarks>
        /// The maximum number of input tokens is 2000.
        /// </remarks>
        public static Model TTS_GPT_4o_Mini { get; } = new("gpt-4o-mini-tts", "openai");

        /// <summary>
        /// Whisper is a general-purpose speech recognition model, trained on a large dataset of diverse audio.
        /// You can also use it as a multitask model to perform multilingual speech recognition as well as speech translation and language identification.
        /// </summary>
        public static Model Whisper1 { get; } = new("whisper-1", "openai");

        /// <summary>
        /// GPT-4o Transcribe is a speech-to-text model that uses GPT-4o to transcribe audio.
        /// It offers improvements to word error rate and better language recognition and accuracy compared to original Whisper models.
        /// Use it for more accurate transcripts.
        /// </summary>
        public static Model Transcribe_GPT_4o { get; } = new("gpt-4o-transcribe", "openai");

        /// <summary>
        /// GPT-4o mini Transcribe is a speech-to-text model that uses GPT-4o mini to transcribe audio.
        /// It offers improvements to word error rate and better language recognition and accuracy compared to original Whisper models.
        /// Use it for more accurate transcripts.
        /// </summary>
        public static Model Transcribe_GPT_4o_Mini { get; } = new("gpt-4o-mini-transcribe", "openai");

        #endregion Audio Models

        #region Image Models

        /// <summary>
        /// GPT Image 1 is our new state-of-the-art image generation model.
        /// It is a natively multimodal language model that accepts both text and image inputs, and produces image outputs.
        /// </summary>
        public static Model GPT_Image_1 { get; } = new("gpt-image-1", "openai");

        /// <summary>
        /// DALL·E is an AI system that creates realistic images and art from a natural language description.
        /// DALL·E 3 currently supports the ability, given a prompt, to create a new image with a specific size.
        /// </summary>
        public static Model DallE_3 { get; } = new("dall-e-3", "openai");

        /// <summary>
        /// DALL·E is an AI system that creates realistic images and art from a natural language description.
        /// Older than DALL·E 3, DALL·E 2 offers more control in prompting and more requests at once.
        /// </summary>
        public static Model DallE_2 { get; } = new("dall-e-2", "openai");

        #endregion Image Models

        #region Specialized Models

        /// <summary>
        /// GPT-5.1-Codex-Max is purpose-built for agentic coding.
        /// It's only available in the Responses API.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 tokens<br/>
        /// - Max Output Tokens: 128,000 tokens
        /// </remarks>
        public static Model GPT5_1_CodexMax { get; } = new("gpt-5.1-codex-max", "openai");

        /// <summary>
        /// GPT-5.1-Codex is a version of GPT-5 optimized for agentic coding tasks in Codex or similar environments.
        /// It's available in the Responses API
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 tokens<br/>
        /// - Max Output Tokens: 128,000 tokens
        /// </remarks>
        public static Model GPT5_1_Codex { get; } = new("gpt-5.1-codex", "openai");

        /// <summary>
        /// GPT-5.1 Codex mini is a smaller, more cost-effective, less-capable version of GPT-5.1-Codex.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 tokens<br/>
        /// - Max Output Tokens: 128,000 tokens
        /// </remarks>
        public static Model GPT5_1_CodexMini { get; } = new("gpt-5.1-codex-mini", "openai");

        /// <summary>
        /// GPT-5-Codex is a version of GPT-5 optimized for agentic coding tasks in Codex or similar environments.
        /// It's available in the Responses API only and the underlying model snapshot will be regularly updated.
        /// </summary>
        /// <remarks>
        /// - Context Window: 400,000 tokens<br/>
        /// - Max Output Tokens: 128,000 tokens
        /// </remarks>
        public static Model GPT5_Codex { get; } = new("gpt-5-codex", "openai");

        /// <summary>
        /// codex-mini-latest is a fine-tuned version of o4-mini specifically for use in Codex CLI.
        /// </summary>
        /// <remarks>
        /// - Context Window: 200,000 tokens<br/>
        /// - Max Output Tokens: 100,000 tokens
        /// </remarks>
        public static Model Codex_Mini_Latest { get; } = new("codex-mini-latest", "openai");

        #endregion Specialized Models

        #region Open Weight Models

        /// <summary>
        /// gpt-oss-120b is our most powerful open-weight model, which fits into a single H100 GPU (117B parameters with 5.1B active parameters).
        /// </summary>
        /// <remarks>
        /// - Context Window: 131,072 context window<br/>
        /// - Max Output Tokens: 131,072 max output tokens
        /// </remarks>
        public static Model GPT_OSS_120B { get; } = new("gpt-oss-120b", "openai");

        /// <summary>
        /// gpt-oss-20b is our medium-sized open-weight model for low latency, local, or specialized use-cases (21B parameters with 3.6B active parameters).
        /// </summary>
        /// <remarks>
        /// - Context Window: 131,072 context window<br/>
        /// - Max Output Tokens: 131,072 max output tokens
        /// </remarks>
        public static Model GPT_OSS_20B { get; } = new("gpt-oss-20b", "openai");

        #endregion Open Weight Models
    }
}
