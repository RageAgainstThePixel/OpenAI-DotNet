using NUnit.Framework;
using OpenAI.FileTunes;
using System;
using System.IO;

namespace OpenAI.Tests
{
    internal class TestFixture_06_Files
    {
        [Test]
        public void Test_01_UploadFile()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            File.WriteAllText("test.jsonl", new FineTunesTrainingData("I'm a", "learning language model"));
            Assert.IsTrue(File.Exists("test.jsonl"));
            var result = api.FilesEndpoint.UploadFileAsync("test.jsonl", "fine-tune").Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.FileName == "test.jsonl");
            Console.WriteLine($"{result.Id} -> {result.Object}");

            File.Delete("test.jsonl");
            Assert.IsFalse(File.Exists("test.jsonl"));
        }

        [Test]
        public void Test_02_ListFiles()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var result = api.FilesEndpoint.ListFilesAsync().Result;

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            foreach (var file in result)
            {
                Console.WriteLine($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
            }
        }

        [Test]
        public void Test_03_DownloadFile()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var files = api.FilesEndpoint.ListFilesAsync().Result;

            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            var testFileData = files[0];
            var result = api.FilesEndpoint.DownloadFileAsync(testFileData, Directory.GetCurrentDirectory()).Result;

            Assert.IsNotNull(result);
            Console.WriteLine(result);
            Assert.IsTrue(File.Exists(result));

            File.Delete(result);
            Assert.IsFalse(File.Exists(result));
        }

        [Test]
        public void Test_04_DeleteFiles()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var files = api.FilesEndpoint.ListFilesAsync().Result;
            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            foreach (var file in files)
            {
                var result = api.FilesEndpoint.DeleteFileAsync(file).Result;
                Assert.IsTrue(result);
                Console.WriteLine($"{file.Id} -> deleted");
            }

            files = api.FilesEndpoint.ListFilesAsync().Result;
            Assert.IsNotNull(files);
            Assert.IsEmpty(files);
        }
    }
}
