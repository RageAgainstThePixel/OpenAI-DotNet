using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_08_Files
    {
        [Test]
        public async Task Test_01_UploadFile()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            File.WriteAllText("test.jsonl", new FineTuningTrainingData("I'm a", "learning language model"));
            Assert.IsTrue(File.Exists("test.jsonl"));
            var result = await api.FilesEndpoint.UploadFileAsync("test.jsonl", "fine-tune");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.FileName == "test.jsonl");
            Console.WriteLine($"{result.Id} -> {result.Object}");

            File.Delete("test.jsonl");
            Assert.IsFalse(File.Exists("test.jsonl"));
        }

        [Test]
        public async Task Test_02_ListFiles()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var result = await api.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            foreach (var file in result)
            {
                var fileInfo = await api.FilesEndpoint.GetFileInfoAsync(file);
                Assert.IsNotNull(fileInfo);
                Console.WriteLine($"{fileInfo.Id} -> {fileInfo.Object}: {fileInfo.FileName} | {fileInfo.Size} bytes");
            }
        }

        [Test]
        public async Task Test_03_DownloadFile()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var files = await api.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            var testFileData = files[0];
            var result = await api.FilesEndpoint.DownloadFileAsync(testFileData, Directory.GetCurrentDirectory());

            Assert.IsNotNull(result);
            Console.WriteLine(result);
            Assert.IsTrue(File.Exists(result));

            File.Delete(result);
            Assert.IsFalse(File.Exists(result));
        }

        [Test]
        public async Task Test_04_DeleteFiles()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FilesEndpoint);

            var files = await api.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            foreach (var file in files)
            {
                var result = await api.FilesEndpoint.DeleteFileAsync(file);
                Assert.IsTrue(result);
                Console.WriteLine($"{file.Id} -> deleted");
            }

            files = await api.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(files);
            Assert.IsEmpty(files);
        }
    }
}
