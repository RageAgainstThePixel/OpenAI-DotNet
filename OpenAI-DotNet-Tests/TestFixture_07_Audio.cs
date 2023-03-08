using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_07_Audio
    {
        [Test]
        public async Task Test_1_Transcription()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("..\\..\\..\\Assets\\T3mt39YrlyLoq8laHSdf.mp3");
            var request = new AudioTranscriptionRequest(transcriptionAudio, language: "en");
            var result = await api.AudioEndpoint.CreateTranscriptionAsync(request);
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task Test_2_Translation()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.AudioEndpoint);
            var translationAudio = Path.GetFullPath("..\\..\\..\\Assets\\Ja-botchan_1-1_1-2.mp3");
            var request = new AudioTranslationRequest(Path.GetFullPath(translationAudio));
            var result = await api.AudioEndpoint.CreateTranslationAsync(request);
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }
    }
}