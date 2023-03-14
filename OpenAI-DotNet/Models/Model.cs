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
        /// <param name="id"></param>
        public Model(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Allows a model to be implicitly cast to the string of its id.
        /// </summary>
        /// <param name="model">The <see cref="Model"/> to cast to a string.</param>
        public static implicit operator string(Model model) => model.Id;

        /// <summary>
        /// Allows a string to be implicitly cast as a <see cref="Model"/>
        /// </summary>
        public static implicit operator Model(string name) => new Model(name);

        /// <inheritdoc />
        public override string ToString() => Id;

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; private set; }

        [JsonInclude]
        [JsonPropertyName("permissions")]
        public IReadOnlyList<Permission> Permissions { get; private set; }

        [JsonInclude]
        [JsonPropertyName("root")]
        public string Root { get; private set; }

        [JsonInclude]
        [JsonPropertyName("parent")]
        public string Parent { get; private set; }

        /// <summary>
        /// More capable than any GPT-3.5 model, able to do more complex tasks, and optimized for chat. Will be updated with our latest model iteration.
        /// </summary>
        public static Model GPT4 { get; } = new Model("gpt-4") { OwnedBy = "openai" };

        /// <summary>
        /// Same capabilities as the base gpt-4 mode but with 4x the context length. Will be updated with our latest model iteration.
        /// </summary>
        public static Model GPT4_32K { get; } = new Model("gpt-4-32k") { OwnedBy = "openai" };

        /// <summary>
        /// Because gpt-3.5-turbo performs at a similar capability to text-davinci-003 but at 10%
        /// the price per token, we recommend gpt-3.5-turbo for most use cases.
        /// </summary>
        public static Model GPT3_5_Turbo { get; } = new Model("gpt-3.5-turbo") { OwnedBy = "openai" };

        /// <summary>
        /// The most powerful, largest engine available, although the speed is quite slow.<para/>
        /// Good at: Complex intent, cause and effect, summarization for audience
        /// </summary>
        public static Model Davinci { get; } = new Model("text-davinci-003") { OwnedBy = "openai" };

        /// <summary>
        /// The 2nd most powerful engine, a bit faster than <see cref="Davinci"/>, and a bit faster.<para/>
        /// Good at: Language translation, complex classification, text sentiment, summarization.
        /// </summary>
        public static Model Curie { get; } = new Model("text-curie-001") { OwnedBy = "openai" };

        /// <summary>
        /// The 2nd fastest engine, a bit more powerful than <see cref="Ada"/>, and a bit slower.<para/>
        /// Good at: Moderate classification, semantic search classification
        /// </summary>
        public static Model Babbage { get; } = new Model("text-babbage-001") { OwnedBy = "openai" };

        /// <summary>
        /// The smallest, fastest engine available, although the quality of results may be poor.<para/>
        /// Good at: Parsing text, simple classification, address correction, keywords
        /// </summary>
        public static Model Ada { get; } = new Model("text-ada-001") { OwnedBy = "openai" };

        /// <summary>
        /// The default model for <see cref="Embeddings.EmbeddingsEndpoint"/>.
        /// </summary>
        public static Model Embedding_Ada_002 { get; } = new Model("text-embedding-ada-002") { OwnedBy = "openai" };

        /// <summary>
        /// The default model for <see cref="Audio.AudioEndpoint"/>.
        /// </summary>
        public static Model Whisper1 { get; } = new Model("whisper-1") { OwnedBy = "openai" };

        /// <summary>
        /// The default model for <see cref="Moderations.ModerationsEndpoint"/>.
        /// </summary>
        public static Model Moderation_Latest { get; } = new Model("text-moderation-latest") { OwnedBy = "openai" };
    }
}
