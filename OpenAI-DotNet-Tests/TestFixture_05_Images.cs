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
        [Test]
        public async Task Test_01_01_GenerateImages()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);
            var request = new ImageGenerationRequest("A house riding a velociraptor", Model.DallE_3);
            var imageResults = await OpenAIClient.ImagesEndPoint.GenerateImageAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }

        [Test]
        public async Task Test_01_02_GenerateImages_B64_Json()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var request = new ImageGenerationRequest("A house riding a velociraptor", Model.DallE_2, responseFormat: ResponseFormat.B64_Json);
            var imageResults = await OpenAIClient.ImagesEndPoint.GenerateImageAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }

        [Test]
        public async Task Test_02_01_CreateImageEdit()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var maskAssetPath = Path.GetFullPath("../../../Assets/image_edit_mask.png");

            var request = new ImageEditRequest(imageAssetPath, maskAssetPath, "A sunlit indoor lounge area with a pool containing a flamingo", size: ImageSize.Small);
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageEditAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }

        [Test]
        public async Task Test_02_02_CreateImageEdit_B64_Json()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var maskAssetPath = Path.GetFullPath("../../../Assets/image_edit_mask.png");

            var request = new ImageEditRequest(imageAssetPath, maskAssetPath, "A sunlit indoor lounge area with a pool containing a flamingo", size: ImageSize.Small, responseFormat: ResponseFormat.B64_Json);
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageEditAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }

        [Test]
        public async Task Test_03_01_CreateImageVariation()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("../../../Assets/image_edit_original.png");
            var request = new ImageVariationRequest(imageAssetPath, size: ImageSize.Small);
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
            var request = new ImageVariationRequest(imageAssetPath, size: ImageSize.Small, responseFormat: ResponseFormat.B64_Json);
            var imageResults = await OpenAIClient.ImagesEndPoint.CreateImageVariationAsync(request);

            Assert.IsNotNull(imageResults);
            Assert.NotZero(imageResults.Count);

            foreach (var image in imageResults)
            {
                Assert.IsNotNull(image);
                Console.WriteLine(image);
            }
        }
    }
}
