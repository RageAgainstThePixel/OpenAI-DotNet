// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace OpenAI.Files
{
    public class FilePurpose
    {
        public static readonly FilePurpose Assistants = "assistants";
        public static readonly FilePurpose Batch = "batch";
        public static readonly FilePurpose FineTune = "fine-tune";
        public static readonly FilePurpose Vision = "vision";

        public FilePurpose(string purpose) => Value = purpose;

        public string Value { get; }

        public override string ToString() => Value;

        public static implicit operator FilePurpose(string purpose) => new(purpose);

        public static implicit operator string(FilePurpose purpose) => purpose?.ToString();
    }
}
