// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Images;
using OpenAI.Tests.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_00_02_Tools : AbstractTestFixture
    {
        [Test]
        public void Test_01_GetTools()
        {
            var tools = Tool.GetAllAvailableTools(forceUpdate: true, clearCache: true).ToList();
            Assert.IsNotNull(tools);
            Assert.IsNotEmpty(tools);
            tools.Add(Tool.GetOrCreateTool(OpenAIClient.ImagesEndPoint, nameof(ImagesEndpoint.GenerateImageAsync)));
            var json = JsonSerializer.Serialize(tools, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
        }

        [Test]
        public async Task Test_02_Tool_Funcs()
        {
            var tools = new List<Tool>
            {
                Tool.FromFunc("test_func", Function),
                Tool.FromFunc<string, string, string>("test_func_with_args", FunctionWithArgs),
                Tool.FromFunc("test_func_weather", () => WeatherService.GetCurrentWeatherAsync("my location", WeatherService.WeatherUnit.Celsius))
            };

            var json = JsonSerializer.Serialize(tools, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
            Assert.IsNotNull(tools);
            var tool = tools[0];
            Assert.IsNotNull(tool);
            var result = tool.InvokeFunction<string>();
            Assert.AreEqual("success", result);
            var toolWithArgs = tools[1];
            Assert.IsNotNull(toolWithArgs);
            toolWithArgs.Function.Arguments = new JsonObject
            {
                ["arg1"] = "arg1",
                ["arg2"] = "arg2"
            };
            var resultWithArgs = toolWithArgs.InvokeFunction<string>();
            Assert.AreEqual("arg1 arg2", resultWithArgs);

            var toolWeather = tools[2];
            Assert.IsNotNull(toolWeather);
            var resultWeather = await toolWeather.InvokeFunctionAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultWeather));
            Console.WriteLine(resultWeather);
        }

        private string Function()
        {
            return "success";
        }

        private string FunctionWithArgs(string arg1, string arg2)
        {
            return $"{arg1} {arg2}";
        }

        [Test]
        public void Test_03_Tool_works_when_called_concurrently()
        {
            Assert.Multiple(async () =>
            {
                await Task.WhenAll(
                    Test(1),
                    Test(2),
                    Test(3),
                    Test(4)
                );
            });

            async Task Test(int id)
            {
                var tool = Tool.FromFunc($"myFunc_{id}", () => id, "This func allows reading the local variable id");

                // Delay a little bit to simulate calling OpenAi API:
                await Task.Delay(50);

                var result = tool.InvokeFunction<int>();
                Assert.AreEqual(id, result);
            }
        }
    }
}