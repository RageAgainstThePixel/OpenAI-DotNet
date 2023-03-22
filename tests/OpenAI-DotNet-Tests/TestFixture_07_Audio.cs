using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal sealed class TestFixture_07_Audio : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_TranscriptionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("..\\..\\..\\Assets\\T3mt39YrlyLoq8laHSdf.mp3");
            var request = new AudioTranscriptionRequest(transcriptionAudio, language: "en");
            var result = await this.OpenAIClient.AudioEndpoint.CreateTranscriptionAsync(request);
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task Test_2_TranslationAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.AudioEndpoint);
            var translationAudio = Path.GetFullPath("..\\..\\..\\Assets\\Ja-botchan_1-1_1-2.mp3");
            var request = new AudioTranslationRequest(Path.GetFullPath(translationAudio));
            var result = await this.OpenAIClient.AudioEndpoint.CreateTranslationAsync(request);
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }
    }
}