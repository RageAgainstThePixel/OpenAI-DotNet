// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class LocalShellAction
    {
        public LocalShellAction() { }

        public LocalShellAction(string command,
            IReadOnlyDictionary<string, string> environment = null,
            int? timeoutMilliseconds = null,
            string user = null,
            string workingDirectory = null)
            : this([command], environment, timeoutMilliseconds, user, workingDirectory)
        {
        }

        public LocalShellAction(
            IEnumerable<string> command,
            IReadOnlyDictionary<string, string> environment = null,
            int? timeoutMilliseconds = null,
            string user = null,
            string workingDirectory = null)
        {
            Command = command.ToList();
            Environment = environment ?? new Dictionary<string, string>();
            TimeoutMilliseconds = timeoutMilliseconds;
            User = user;
            WorkingDirectory = workingDirectory;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "exec";

        /// <summary>
        /// The command to run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("command")]
        public IReadOnlyList<string> Command { get; private set; }

        /// <summary>
        /// Environment variables to set for the command.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("env")]
        public IReadOnlyDictionary<string, string> Environment { get; private set; }

        /// <summary>
        /// Optional timeout in milliseconds for the command.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("timeout_ms")]
        public int? TimeoutMilliseconds { get; private set; }

        /// <summary>
        /// Optional user to run the command as.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("user")]
        public string User { get; private set; }

        /// <summary>
        /// Optional working directory to run the command in.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("working_directory")]
        public string WorkingDirectory { get; private set; }
    }
}
