﻿using NUnit.Framework;
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
        public async Task Test_1_GenerateImages()
        {
            Assert.IsNotNull(OpenAIClient.ImagesEndPoint);

            var request = new ImageGenerationRequest("A house riding a velociraptor", Model.DallE_2);
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
        public async Task Test_2_GenerateImages_B64_Json()
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
        public async Task Test_3_GenerateImageEdits()
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
        public async Task Test_4_GenerateImageEdits_B64_Json()
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
        public async Task Test_5_GenerateImageVariations()
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
        public async Task Test_6_GenerateImageVariations_B64_Json()
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
