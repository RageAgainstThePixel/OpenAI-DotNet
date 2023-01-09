using NUnit.Framework;
using System.IO;

namespace OpenAI.Tests
{
    public class AuthTests
    {
        [SetUp]
        public void Setup()
        {
            File.WriteAllText(".openai", "OPENAI_KEY=pk-test12");
            Assert.IsTrue(File.Exists(".openai"));
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
        public void GetAuthFromNonExistentFile()
        {
            var auth = OpenAIAuthentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void GetDefault()
        {
            var auth = OpenAIAuthentication.Default;
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
        }

        [Test]
        public void Authentication()
        {
            var defaultAuth = OpenAIAuthentication.Default;
            var manualAuth = new OpenAIAuthentication("pk-testAA");
            var api = new OpenAIClient();
            var shouldBeDefaultAuth = api.OpenAIAuthentication;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);

            OpenAIAuthentication.Default = new OpenAIAuthentication("pk-testAA");
            api = new OpenAIClient();
            var shouldBeManualAuth = api.OpenAIAuthentication;
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

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(".openai"))
            {
                File.Delete(".openai");
            }

            Assert.IsFalse(File.Exists(".openai"));
        }
    }
}
