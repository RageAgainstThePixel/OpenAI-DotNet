using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Models
{
    /// <summary>
    /// Represents a language model.
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

        [JsonConstructor]
        public Model(
            string id,
            string @object,
            string ownedBy,
            IReadOnlyList<Permission> permissions,
            string root, string parent) : this(id)
        {
            Object = @object;
            OwnedBy = ownedBy;
            Permissions = permissions;
            Root = root;
            Parent = parent;
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

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; private set; }

        [JsonPropertyName("permissions")]
        public IReadOnlyList<Permission> Permissions { get; }

        [JsonPropertyName("root")]
        public string Root { get; }

        [JsonPropertyName("parent")]
        public string Parent { get; }

        /// <summary>
        /// The default Model to use in the case no other is specified.  Defaults to <see cref="Davinci"/>
        /// </summary>
        public static Model Default => Davinci;

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
    }
}
