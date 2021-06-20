using NUnit.Framework;
using OpenAI;
using System.IO;

namespace OpenAI_Tests
{
    public class AuthTests
    {
        [SetUp]
        public void Setup()
        {
            File.WriteAllText(".openai", "OPENAI_KEY=pk-test12");
        }

        [Test]
        public void GetAuthFromEnv()
        {
            var auth = OpenAIAuthentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
        }

        [Test]
        public void GetAuthFromFile()
        {
            var auth = OpenAIAuthentication.LoadFromDirectory();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-test12", auth.ApiKey);
        }

        [Test]
        public void GetAuthFromNonExistantFile()
        {
            var auth = OpenAIAuthentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void GetDefault()
        {
            var auth = OpenAIAuthentication.Default;
            var envAuth = OpenAIAuthentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual(envAuth.ApiKey, auth.ApiKey);
        }

        [Test]
        public void TestHelper()
        {
            var defaultAuth = OpenAIAuthentication.Default;
            var manualAuth = new OpenAIAuthentication("pk-testAA");
            var api = new OpenAIClient();
            var shouldBeDefaultAuth = api.Auth;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);

            OpenAIAuthentication.Default = new OpenAIAuthentication("pk-testAA");
            api = new OpenAIClient();
            var shouldBeManualAuth = api.Auth;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);

            OpenAIAuthentication.Default = defaultAuth;
        }

        [Test]
        public void GetKey()
        {
            var auth = new OpenAIAuthentication("pk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testAA", auth.ApiKey);
        }

        [Test]
        public void ParseKey()
        {
            var auth = new OpenAIAuthentication("pk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testAA", auth.ApiKey);
            auth = "pk-testCC";
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testCC", auth.ApiKey);

            auth = new OpenAIAuthentication("sk-testBB");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testBB", auth.ApiKey);
        }

    }
}
