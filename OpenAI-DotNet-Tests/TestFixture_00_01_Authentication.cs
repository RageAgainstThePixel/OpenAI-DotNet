// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenAI.Tests
{
    internal class TestFixture_00_01_Authentication
    {
        [SetUp]
        public void Setup()
        {
            var authJson = new AuthInfo("sk-test12", "org-testOrg", "proj_testProject");
            var authText = JsonSerializer.Serialize(authJson);
            File.WriteAllText(OpenAIAuthentication.CONFIG_FILE, authText);
            Assert.IsTrue(File.Exists(OpenAIAuthentication.CONFIG_FILE));
        }

        [Test]
        public void Test_01_GetAuthFromEnv()
        {
            var auth = OpenAIAuthentication.LoadFromEnv();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);

            auth = OpenAIAuthentication.LoadFromEnv("org-testOrg");
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
            Assert.IsNotNull(auth.OrganizationId);
            Assert.IsNotEmpty(auth.OrganizationId);
        }

        [Test]
        public void Test_02_GetAuthFromFile()
        {
            var auth = OpenAIAuthentication.LoadFromPath(Path.GetFullPath(OpenAIAuthentication.CONFIG_FILE));
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-test12", auth.ApiKey);
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testOrg", auth.OrganizationId);
            Assert.IsNotNull(auth.ProjectId);
            Assert.AreEqual("proj_testProject", auth.ProjectId);
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
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testOrg", auth.OrganizationId);
        }

        [Test]
        public void Test_05_Authentication()
        {
            var defaultAuth = OpenAIAuthentication.Default;
            var manualAuth = new OpenAIAuthentication("sk-testAA", "org-testAA", "proj_testProject");
            using var api = new OpenAIClient();
            var shouldBeDefaultAuth = api.OpenAIAuthentication;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.IsNotNull(shouldBeDefaultAuth.OrganizationId);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.OrganizationId, shouldBeDefaultAuth.OrganizationId);
            Assert.AreEqual(defaultAuth.ProjectId, shouldBeDefaultAuth.ProjectId);

            OpenAIAuthentication.Default = new OpenAIAuthentication("sk-testAA", "org-testAA", "proj_testProject");
            using var api2 = new OpenAIClient();
            var shouldBeManualAuth = api2.OpenAIAuthentication;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.IsNotNull(shouldBeManualAuth.OrganizationId);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.OrganizationId, shouldBeManualAuth.OrganizationId);
            Assert.AreEqual(manualAuth.ProjectId, shouldBeDefaultAuth.ProjectId);

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
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testAA", auth.OrganizationId);
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

        [Test]
        public void Test_11_AzureConfigurationSettings()
        {
            var auth = new OpenAIAuthentication("testKeyAaBbCcDd");
            var settings = new OpenAIClientSettings(resourceName: "test-resource", deploymentId: "deployment-id-test");
            var api = new OpenAIClient(auth, settings);
            Console.WriteLine(api.OpenAIClientSettings.BaseRequest);
            Console.WriteLine(api.OpenAIClientSettings.BaseRequestUrlFormat);
            Assert.AreEqual("https://test-resource.openai.azure.com/openai/{0}", api.OpenAIClientSettings.BaseRequestUrlFormat);
        }

        [Test]
        public void Test_12_CustomDomainConfigurationSettings()
        {
            var auth = new OpenAIAuthentication("sess-customIssuedToken");
            var settings = new OpenAIClientSettings(domain: "api.your-custom-domain.com");
            var api = new OpenAIClient(auth, settings);
            Console.WriteLine(api.OpenAIClientSettings.BaseRequest);
            Console.WriteLine(api.OpenAIClientSettings.BaseRequestUrlFormat);
            Assert.AreEqual("https://api.your-custom-domain.com/v1/{0}", api.OpenAIClientSettings.BaseRequestUrlFormat);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(OpenAIAuthentication.CONFIG_FILE))
            {
                File.Delete(OpenAIAuthentication.CONFIG_FILE);
            }

            Assert.IsFalse(File.Exists(OpenAIAuthentication.CONFIG_FILE));
        }
    }
}
