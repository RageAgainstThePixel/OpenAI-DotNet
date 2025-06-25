﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A tool that controls a virtual computer. Learn more about the computer tool.
    /// </summary>
    public sealed class ComputerUsePreviewTool : ITool
    {
        public static implicit operator Tool(ComputerUsePreviewTool computerUsePreviewTool) => new(computerUsePreviewTool as ITool);

        public ComputerUsePreviewTool() { }

        public ComputerUsePreviewTool(int displayHeight, int displayWidth, string environment)
        {
            DisplayHeight = displayHeight;
            DisplayWidth = displayWidth;
            Environment = environment;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "computer_use_preview";

        /// <summary>
        /// The height of the computer display.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("display_height")]
        public int DisplayHeight { get; private set; }

        /// <summary>
        /// The width of the computer display.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("display_width")]
        public int DisplayWidth { get; private set; }

        /// <summary>
        /// The type of computer environment to control.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("environment")]
        public string Environment { get; private set; }
    }
}
