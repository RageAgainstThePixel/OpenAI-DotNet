// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System;

namespace OpenAI.Tests
{
    internal class TestFixture_00_02_Extensions
    {
        [Test]
        public void Test_01_Tools()
        {
            var tools = Tool.GetAllAvailableTools();

            for (var i = 0; i < tools.Count; i++)
            {
                var tool = tools[i];

                if (tool.Type != "function")
                {
                    Console.Write($"  \"{tool.Type}\"");
                }
                else
                {
                    Console.Write($"  \"{tool.Function.Name}\"");
                }

                if (tool.Function?.Parameters != null)
                {
                    Console.Write($": {tool.Function.Parameters}");
                }

                if (i < tools.Count - 1)
                {
                    Console.Write(",\n");
                }
            }
        }
    }
}