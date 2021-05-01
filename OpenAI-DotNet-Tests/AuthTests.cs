using NUnit.Framework;
using OpenAI_DotNet;
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
            var auth = Authentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
        }

        [Test]
        public void GetAuthFromFile()
        {
            var auth = Authentication.LoadFromDirectory();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-test12", auth.ApiKey);
        }

        [Test]
        public void GetAuthFromNonExistantFile()
        {
            var auth = Authentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void GetDefault()
        {
            var auth = Authentication.Default;
            var envAuth = Authentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual(envAuth.ApiKey, auth.ApiKey);
        }

        [Test]
        public void TestHelper()
        {
            var defaultAuth = Authentication.Default;
            var manualAuth = new Authentication("pk-testAA");
            var api = new OpenAI();
            var shouldBeDefaultAuth = api.Auth;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);

            Authentication.Default = new Authentication("pk-testAA");
            api = new OpenAI();
            var shouldBeManualAuth = api.Auth;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);

            Authentication.Default = defaultAuth;
        }

        [Test]
        public void GetKey()
        {
            var auth = new Authentication("pk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testAA", auth.ApiKey);
        }

        [Test]
        public void ParseKey()
        {
            var auth = new Authentication("pk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testAA", auth.ApiKey);
            auth = "pk-testCC";
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("pk-testCC", auth.ApiKey);

            auth = new Authentication("sk-testBB");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testBB", auth.ApiKey);
        }

    }
}
