using NUnit.Framework;
using System;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI.Tests
{
    internal class TestFixture_00_Authentication
    {
        [SetUp]
        public void Setup()
        {
            var authJson = new AuthInfo("sk-test12", "org-testOrg");
            var authText = JsonSerializer.Serialize(authJson);
            File.WriteAllText(".openai", authText);
            Assert.IsTrue(File.Exists(".openai"));
        }

        [Test]
        public void Test_01_GetAuthFromEnv()
        {
            var auth = OpenAIAuthentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
            Assert.IsNull(auth.Organization);

            auth = OpenAIAuthentication.LoadFromEnv("org-testOrg");
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
            Assert.IsNotNull(auth.Organization);
            Assert.IsNotEmpty(auth.Organization);
        }

        [Test]
        public void Test_02_GetAuthFromFile()
        {
            var auth = OpenAIAuthentication.LoadFromDirectory();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-test12", auth.ApiKey);
            Assert.IsNotNull(auth.Organization);
            Assert.AreEqual("org-testOrg", auth.Organization);
        }

        [Test]
        public void Test_03_GetAuthFromNonExistentFile()
        {
            var auth = OpenAIAuthentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void Test_04_GetDefault()
        {
            var auth = OpenAIAuthentication.Default;
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-test12", auth.ApiKey);
            Assert.IsNotNull(auth.Organization);
            Assert.AreEqual("org-testOrg", auth.Organization);
        }

        [Test]
        public void Test_05_Authentication()
        {
            var defaultAuth = OpenAIAuthentication.Default;
            var manualAuth = new OpenAIAuthentication("sk-testAA", "org-testAA");
            var api = new OpenAIClient();
            var shouldBeDefaultAuth = api.OpenAIAuthentication;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.IsNotNull(shouldBeDefaultAuth.Organization);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.Organization, shouldBeDefaultAuth.Organization);

            OpenAIAuthentication.Default = new OpenAIAuthentication("sk-testAA", "org-testAA");
            api = new OpenAIClient();
            var shouldBeManualAuth = api.OpenAIAuthentication;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.IsNotNull(shouldBeManualAuth.Organization);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.Organization, shouldBeManualAuth.Organization);

            OpenAIAuthentication.Default = defaultAuth;
        }

        [Test]
        public void Test_06_GetKey()
        {
            var auth = new OpenAIAuthentication("sk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testAA", auth.ApiKey);
        }

        [Test]
        public void Test_07_GetKeyFailed()
        {
            OpenAIAuthentication auth = null;

            try
            {
                auth = new OpenAIAuthentication("fail-key");
            }
            catch (InvalidCredentialException)
            {
                Assert.IsNull(auth);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, $"Expected exception {nameof(InvalidCredentialException)} but got {e.GetType().Name}");
            }
        }

        [Test]
        public void Test_08_ParseKey()
        {
            var auth = new OpenAIAuthentication("sk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testAA", auth.ApiKey);
            auth = "sk-testCC";
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testCC", auth.ApiKey);

            auth = new OpenAIAuthentication("sk-testBB");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testBB", auth.ApiKey);
        }

        [Test]
        public void Test_09_GetOrganization()
        {
            var auth = new OpenAIAuthentication("sk-testAA", "org-testAA");
            Assert.IsNotNull(auth.Organization);
            Assert.AreEqual("org-testAA", auth.Organization);
        }

        [Test]
        public void Test_10_GetOrgFailed()
        {
            OpenAIAuthentication auth = null;

            try
            {
                auth = new OpenAIAuthentication("sk-testAA", "fail-org");
            }
            catch (InvalidCredentialException)
            {
                Assert.IsNull(auth);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, $"Expected exception {nameof(InvalidCredentialException)} but got {e.GetType().Name}");
            }
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
