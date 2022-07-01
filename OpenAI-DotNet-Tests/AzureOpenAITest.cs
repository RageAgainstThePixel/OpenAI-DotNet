using NUnit.Framework;
using OpenAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI_Tests
{
    public class AzureOpenAITest
    {

        private readonly string prompts = "One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight";

        [SetUp]
        public void Setup()
        {
            File.WriteAllText(".azureopenai", "Endpoint=xxxx;Key=xxxx;Deployment=xxx");
        }

        public AzureOpenAIClient LoadAzureOpenAIClient()
        {
            var azureOpenAiConfig = string.Join("\n", File.ReadAllLines(".azureopenai")).Split(";").Select(x => x.Split("=")).ToDictionary(x => x[0], y => y[1]);

            return new AzureOpenAIClient(azureOpenAiConfig["Endpoint"], new OpenAIAuthentication(azureOpenAiConfig["Key"]), new Engine(azureOpenAiConfig["Deployment"]));
        }

        //[Test]
        public async Task TestAzureOpenAiEngines()
        {
            var aoaiClient = LoadAzureOpenAIClient();
            Assert.IsNotNull(aoaiClient.CompletionEndpoint);

            var result = await aoaiClient.CompletionEndpoint.CreateCompletionAsync(prompts, temperature: 0.1, max_tokens: 5, numOutputs: 5);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Completions);
            Assert.NotZero(result.Completions.Count);
            Assert.That(result.Completions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
            Console.WriteLine(result);
        }
    }
}
