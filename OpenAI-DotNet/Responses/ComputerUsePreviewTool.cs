// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A tool that controls a virtual computer. Learn more about the computer tool.
    /// </summary>
    public sealed class ComputerUsePreviewTool : ITool
    {
        public static implicit operator Tool(ComputerUsePreviewTool computerUsePreviewTool) => new(computerUsePreviewTool as ITool);

        public ComputerUsePreviewTool(int displayHeight, int displayWidth, string environment)
        {
            DisplayHeight = displayHeight;
            DisplayWidth = displayWidth;
            Environment = environment;
        }

        [JsonPropertyName("type")]
        public string Type => "computer_use_preview";

        /// <summary>
        /// The height of the computer display.
        /// </summary>
        [JsonPropertyName("display_height")]
        public int DisplayHeight { get; }

        /// <summary>
        /// The width of the computer display.
        /// </summary>
        [JsonPropertyName("display_width")]
        public int DisplayWidth { get; }

        /// <summary>
        /// The type of computer environment to control.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; }
    }
}
