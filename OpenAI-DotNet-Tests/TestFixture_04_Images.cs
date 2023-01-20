using NUnit.Framework;
using OpenAI.Images;
using System;
using System.IO;

namespace OpenAI.Tests
{
    internal class TestFixture_04_Images
    {
        [Test]
        public void Test_1_GenerateImages()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ImagesEndPoint);

            var results = api.ImagesEndPoint.GenerateImageAsync("A house riding a velociraptor", 1, ImageSize.Small).Result;

            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        [Test]
        public void Test_2_GenerateImageEdits()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("..\\..\\..\\Assets\\image_edit_original.png");
            var maskAssetPath = Path.GetFullPath("..\\..\\..\\Assets\\image_edit_mask.png");

            var results = api.ImagesEndPoint.CreateImageEditAsync(Path.GetFullPath(imageAssetPath), Path.GetFullPath(maskAssetPath), "A sunlit indoor lounge area with a pool containing a flamingo", 1, ImageSize.Small).Result;

            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        [Test]
        public void Test_3_GenerateImageVariations()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ImagesEndPoint);

            var imageAssetPath = Path.GetFullPath("..\\..\\..\\Assets\\image_edit_original.png");

            var results = api.ImagesEndPoint.CreateImageVariationAsync(imageAssetPath, 1, ImageSize.Small).Result;

            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
