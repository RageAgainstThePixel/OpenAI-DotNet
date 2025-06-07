// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Images;
using OpenAI.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_05_Images : AbstractTestFixture
    {
        private string testDirectory;

        [OneTimeSetUp]
        public void Setup()
        {
            testDirectory = Path.GetFullPath($"../../../Assets/Tests/{nameof(TestFixture_05_Images)}");

            if (!Directory.Exists(testDirectory))
            {
                Directory.CreateDirectory(testDirectory);
            }
        }

        [Test]
        public async Task Test_01_01_GenerateImages()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);
            var request = new ImageGenerationRequest("A house riding a velociraptor", outputFormat: "jpeg");
            var imageResults = await OpenAIClient.ImagesEndPoint.GenerateImageAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Assert.IsFalse(string.IsNullOrWhiteSpace(image.B64_Json));
                var imageBytes = Convert.FromBase64String(image.B64_Json);
                Assert.IsNotNull(imageBytes);
                var path = Path.Combine(testDirectory, $"{nameof(Test_01_01_GenerateImages)}-{DateTime.UtcNow:yyyyMMddHHmmss}.jpeg");
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                Console.WriteLine(path);
            }
        }

        [Test]
        public async Task Test_02_01_CreateImageEdit()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var maskAssetPath = Path.GetFullPath("../../../Assets/image_edit_mask.png");

            var request = new ImageEditRequest(
                prompt: "A sunlit indoor lounge area with a pool containing a flamingo",
                imagePath: imageAssetPath,
                maskPath: maskAssetPath,
                model: Model.GPT_Image_1);
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageEditAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Assert.IsFalse(string.IsNullOrWhiteSpace(image.B64_Json));
                var imageBytes = Convert.FromBase64String(image.B64_Json);
                Assert.IsNotNull(imageBytes);
                var path = Path.Combine(testDirectory, $"{nameof(Test_02_01_CreateImageEdit)}-{DateTime.UtcNow:yyyyMMddHHmmss}.png");
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                Console.WriteLine(path);
            }
        }

        [Test]
        public async Task Test_03_01_CreateImageVariation()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var request = new ImageVariationRequest(imageAssetPath, size: "256x256");
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageVariationAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }

        [Test]
        public async Task Test_03_02_CreateImageVariation_B64_Json()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var request = new ImageVariationRequest(imageAssetPath, size: "256x256", responseFormat: ImageResponseFormat.B64_Json);
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageVariationAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Assert.IsFalse(string.IsNullOrWhiteSpace(image.B64_Json));
                var imageBytes = Convert.FromBase64String(image.B64_Json);
                Assert.IsNotNull(imageBytes);
                var path = Path.Combine(testDirectory, $"{nameof(Test_03_02_CreateImageVariation_B64_Json)}-{DateTime.UtcNow:yyyyMMddHHmmss}.png");
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                Console.WriteLine(path);
            }
        }
    }
}
