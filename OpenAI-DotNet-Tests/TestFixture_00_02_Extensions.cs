// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Extensions;
using OpenAI.Images;
using OpenAI.Responses;
using OpenAI.Tests.StructuredOutput;
using OpenAI.Tests.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_00_02_Extensions : AbstractTestFixture
    {
        [Test]
        public void Test_01_01_01_GetTools()
        {
            var tools = Tool.GetAllAvailableTools(forceUpdate: true, clearCache: true).ToList();
            Assert.IsNotNull(tools);
            Assert.IsNotEmpty(tools);
            tools.Add(Tool.GetOrCreateTool(OpenAIClient.ImagesEndPoint, nameof(ImagesEndpoint.GenerateImageAsync)));
            tools.Add(Tool.GetOrCreateTool(OpenAIClient.AudioEndpoint, nameof(AudioEndpoint.CreateSpeechAsync)));
            tools.ProcessTools<Tool>(null, out var toolList, out _);
            Assert.NotNull(toolList);
            Console.WriteLine(JsonSerializer.Serialize(toolList, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            }));
        }

        [Test]
        public void Test_01_01_02_GetTools()
        {
            var tools = Tool.GetAllAvailableTools(forceUpdate: true, clearCache: true).ToList();
            Assert.IsNotNull(tools);
            Assert.IsNotEmpty(tools);
            tools.Add(Tool.GetOrCreateTool(OpenAIClient.ImagesEndPoint, nameof(ImagesEndpoint.GenerateImageAsync)));
            tools.Add(new CodeInterpreterTool("container_id"));
            tools.Add(new ComputerUsePreviewTool(1024, 768, "browser"));
            tools.Add(new FileSearchTool("vector_store_id"));
            tools.Add(new ImageGenerationTool());
            tools.Add(new LocalShellTool());
            tools.Add(new MCPTool("mcp_server", "https://mcp_server"));
            tools.Add(new WebSearchPreviewTool());
            tools.ProcessTools<ITool>(null, out var toolList, out _);
            Assert.NotNull(toolList);

            Console.WriteLine(JsonSerializer.Serialize(toolList, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            }));
        }

        [Test]
        public async Task Test_01_02_Tool_Funcs()
        {
            var tools = new List<Tool>
            {
                Tool.FromFunc("test_func", Function),
                Tool.FromFunc<DateTime, Vector3, string>("test_func_with_args", FunctionWithArgs),
                Tool.FromFunc("test_func_weather", () => WeatherService.GetCurrentWeatherAsync("my location", WeatherService.WeatherUnit.Celsius)),
                Tool.FromFunc<List<int>, string>("test_func_with_array_args", FunctionWithArrayArgs),
                Tool.FromFunc<string, string>("test_single_return_arg", arg1 => arg1),
                Tool.FromFunc("test_no_specifiers", (string arg1) => arg1)
            };

            var json = JsonSerializer.Serialize(tools, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
            Assert.IsNotNull(tools);
            var tool = tools[0];
            Assert.IsNotNull(tool);
            var toolCall = new ToolCall("toolCall_0", tool.Function.Name);
            var result = tool.InvokeFunction<string>(toolCall);
            Assert.AreEqual("success", result);

            var toolWithArgs = tools[1];
            Assert.IsNotNull(toolWithArgs);
            var testValue = new { arg1 = DateTime.UtcNow, arg2 = Vector3.One };
            toolCall = new ToolCall("toolCall_1", toolWithArgs.Function.Name, JsonSerializer.Serialize(testValue, OpenAIClient.JsonSerializationOptions));
            var resultWithArgs = toolWithArgs.InvokeFunction<string>(toolCall);
            Console.WriteLine(resultWithArgs);

            var toolWeather = tools[2];
            Assert.IsNotNull(toolWeather);
            toolCall = new ToolCall("toolCall_2", toolWeather.Function.Name);
            var resultWeather = await toolWeather.InvokeFunctionAsync(toolCall);
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultWeather));
            Console.WriteLine(resultWeather);

            var toolWithArrayArgs = tools[3];
            Assert.IsNotNull(toolWithArrayArgs);
            var arrayTestValue = new { list = new List<int> { 1, 2, 3, 4, 5 } };
            toolCall = new ToolCall("toolCall_3", toolWithArrayArgs.Function.Name, JsonSerializer.Serialize(arrayTestValue, OpenAIClient.JsonSerializationOptions));
            var resultWithArrayArgs = toolWithArrayArgs.InvokeFunction<string>(toolCall);
            Assert.AreEqual("{\"list\":[1,2,3,4,5]}", resultWithArrayArgs);
            Console.WriteLine(resultWithArrayArgs);

            var toolSingleReturnArg = tools[4];
            Assert.IsNotNull(toolSingleReturnArg);
            var singleReturnArgTestValue = new Dictionary<string, string> { { "arg1", "arg1" } };
            toolCall = new ToolCall("toolCall_4", toolSingleReturnArg.Function.Name, JsonSerializer.Serialize(singleReturnArgTestValue, OpenAIClient.JsonSerializationOptions));
            var resultSingleReturnArg = toolSingleReturnArg.InvokeFunction<string>(toolCall);
            Assert.AreEqual("arg1", resultSingleReturnArg);
            Console.WriteLine(resultSingleReturnArg);

            var toolNoSpecifiers = tools[5];
            Assert.IsNotNull(toolNoSpecifiers);
            toolCall = new ToolCall("toolCall_5", toolNoSpecifiers.Function.Name, JsonSerializer.Serialize(singleReturnArgTestValue, OpenAIClient.JsonSerializationOptions));
            var resultNoSpecifiers = toolNoSpecifiers.InvokeFunction<string>(toolCall);
            Assert.AreEqual("arg1", resultNoSpecifiers);
            Console.WriteLine(resultNoSpecifiers);
        }

        private string Function()
        {
            return "success";
        }

        private string FunctionWithArgs(DateTime arg1, Vector3 arg2)
        {
            return JsonSerializer.Serialize(new { arg1, arg2 }, OpenAIClient.JsonSerializationOptions);
        }

        private string FunctionWithArrayArgs(List<int> list)
        {
            return JsonSerializer.Serialize(new { list }, OpenAIClient.JsonSerializationOptions);
        }

        [Test]
        public void Test_01_03_Tool_works_when_called_concurrently()
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
                var toolCall = new ToolCall($"toolCall_{id}", tool.Function.Name);
                var result = tool.InvokeFunction<int>(toolCall);
                Assert.AreEqual(id, result);
            }
        }

        [Test]
        public void Test_02_01_GenerateJsonSchema()
        {
            JsonSchema mathSchema = typeof(MathResponse);
            Console.WriteLine(mathSchema.ToString());
        }

        [Test]
        public void Test_02_02_GenerateJsonSchema_PrimitiveTypes()
        {
            JsonSchema schema = typeof(TestSchema);
            Console.WriteLine(schema.ToString());
        }

        private class TestSchema
        {
            // test all primitive types can be serialized
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Integer { get; set; }
            public uint UInteger { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public char Char { get; set; }
            public string String { get; set; }
            public DateTime DateTime { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
            public Guid Guid { get; set; }
            // test nullables
            public int? NullInt { get; set; }
            public DateTime? NullDateTime { get; set; }
            public TestEnum TestEnum { get; set; }
            public TestEnum? NullEnum { get; set; }
            public Dictionary<string, object> Dictionary { get; set; }
            public IDictionary<string, int> IntDictionary { get; set; }
            public IReadOnlyDictionary<string, string> StringDictionary { get; set; }
            public Dictionary<string, MathResponse> CustomDictionary { get; set; }
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3,
            Enum4
        }
    }
}
