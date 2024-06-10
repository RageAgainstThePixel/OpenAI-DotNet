// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Files
{
    public class FilePurpose
    {
        public const string Assistants = "assistants";
        public const string Batch = "batch";
        public const string FineTune = "fine-tune";
        public const string Vision = "vision";

        public FilePurpose(string purpose) => Value = purpose;

        public string Value { get; }

        public override string ToString() => Value;

        public static implicit operator FilePurpose(string purpose) => new(purpose);

        public static implicit operator string(FilePurpose purpose) => purpose?.ToString();
    }
}
