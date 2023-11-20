using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_08_Files : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_UploadFile()
        {
            Assert.IsNotNull(OpenAIClient.FilesEndpoint);
            var testData = new Conversation(new List<Message> { new Message(Role.Assistant, "I'm a learning language model") });
            await File.WriteAllTextAsync("test.jsonl", JsonSerializer.Serialize(testData, OpenAIClient.DefaultJsonSerializerOptions));
            Assert.IsTrue(File.Exists("test.jsonl"));
            var result = await OpenAIClient.FilesEndpoint.UploadFileAsync("test.jsonl", "fine-tune");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FileName == "test.jsonl");
            Console.WriteLine($"{result.Id} -> {result.Object}");
            File.Delete("test.jsonl");
            Assert.IsFalse(File.Exists("test.jsonl"));
        }

        [Test]
        public async Task Test_02_ListFiles()
        {
            Assert.IsNotNull(OpenAIClient.FilesEndpoint);
            var result = await OpenAIClient.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            foreach (var file in result)
            {
                var fileInfo = await OpenAIClient.FilesEndpoint.GetFileInfoAsync(file);
                Assert.IsNotNull(fileInfo);
                Console.WriteLine($"{fileInfo.Id} -> {fileInfo.Object}: {fileInfo.FileName} | {fileInfo.Size} bytes");
            }
        }

        [Test]
        public async Task Test_03_DownloadFile()
        {
            Assert.IsNotNull(OpenAIClient.FilesEndpoint);
            var files = await OpenAIClient.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            var testFileData = files[0];
            var result = await OpenAIClient.FilesEndpoint.DownloadFileAsync(testFileData, Directory.GetCurrentDirectory());

            Assert.IsNotNull(result);
            Console.WriteLine(result);
            Assert.IsTrue(File.Exists(result));

            File.Delete(result);
            Assert.IsFalse(File.Exists(result));
        }

        [Test]
        public async Task Test_04_DeleteFiles()
        {
            Assert.IsNotNull(OpenAIClient.FilesEndpoint);
            var files = await OpenAIClient.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);

            foreach (var file in files)
            {
                var result = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
                Assert.IsTrue(result);
                Console.WriteLine($"{file.Id} -> deleted");
            }

            files = await OpenAIClient.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(files);
            Assert.IsEmpty(files);
        }
    }
}
