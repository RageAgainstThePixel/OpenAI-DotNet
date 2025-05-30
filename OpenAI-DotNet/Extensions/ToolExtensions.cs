// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Extensions
{
    internal static class ToolExtensions
    {
        public static void ProcessTools(this IEnumerable<Tool> tools, string toolChoice, out IReadOnlyList<Tool> toolList, out object activeTool)
        {
            toolList = tools?.ToList();

            if (toolList is { Count: > 0 })
            {
                if (string.IsNullOrWhiteSpace(toolChoice))
                {
                    activeTool = "auto";
                }
                else
                {
                    if (!toolChoice.Equals("none") &&
                        !toolChoice.Equals("required") &&
                        !toolChoice.Equals("auto"))
                    {
                        var tool = toolList.FirstOrDefault(t => t.Function.Name.Contains(toolChoice)) ??
                                   throw new ArgumentException($"The specified tool choice '{toolChoice}' was not found in the list of tools");
                        activeTool = new { type = "function", function = new { name = tool.Function.Name } };
                    }
                    else
                    {
                        activeTool = toolChoice;
                    }
                }

                foreach (var tool in toolList)
                {
                    if (tool?.Function?.Arguments != null)
                    {
                        // just in case clear any lingering func args.
                        tool.Function.Arguments = null;
                    }
                }
            }
            else
            {
                activeTool = string.IsNullOrWhiteSpace(toolChoice) ? "none" : toolChoice;
            }
        }
    }
}
