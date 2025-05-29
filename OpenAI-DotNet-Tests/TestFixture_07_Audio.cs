// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_07_Audio : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_01_Transcription_Text()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                responseFormat: AudioResponseFormat.Text,
                temperature: 0.1f,
                language: "en");
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionTextAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_01_02_01_Transcription_Json()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                model: Model.Transcribe_GPT_4o_Mini,
                responseFormat: AudioResponseFormat.Json,
                temperature: 0.1f,
                language: "en",
                include: ["logprobs"]);
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionJsonAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_01_02_02_Transcription_Json_ChunkingStrategy_Auto()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                model: Model.Transcribe_GPT_4o_Mini,
                responseFormat: AudioResponseFormat.Json,
                temperature: 0.1f,
                language: "en",
                chunkingStrategy: ChunkingStrategy.Auto);
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionJsonAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_01_02_03_Transcription_Json_ChunkingStrategy_Specific()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            var chunkStrategy = new ChunkingStrategy(300, 200, 0.5f);
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                model: Model.Transcribe_GPT_4o_Mini,
                responseFormat: AudioResponseFormat.Json,
                temperature: 0.1f,
                language: "en",
                chunkingStrategy: chunkStrategy);
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionJsonAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_01_03_01_Transcription_VerboseJson()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                responseFormat: AudioResponseFormat.Verbose_Json,
                temperature: 0.1f,
                language: "en");
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionJsonAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Duration);
            Assert.IsTrue(response.Language == "english");
            Assert.IsNotNull(response.Segments);
            Assert.IsNotEmpty(response.Segments);
        }

        [Test]
        public async Task Test_01_03_02_Transcription_VerboseJson_WordSimilarities()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var transcriptionAudio = Path.GetFullPath("../../../Assets/T3mt39YrlyLoq8laHSdf.mp3");
            using var request = new AudioTranscriptionRequest(
                audioPath: transcriptionAudio,
                responseFormat: AudioResponseFormat.Verbose_Json,
                timestampGranularity: TimestampGranularity.Word,
                temperature: 0.1f,
                language: "en");
            var response = await OpenAIClient.AudioEndpoint.CreateTranscriptionJsonAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Duration);
            Assert.IsTrue(response.Language == "english");
            Assert.IsNotNull(response.Words);
            Assert.IsNotEmpty(response.Words);
        }

        [Test]
        public async Task Test_02_01_Translation_Text()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var translationAudio = Path.GetFullPath("../../../Assets/Ja-botchan_1-1_1-2.mp3");
            using var request = new AudioTranslationRequest(
                audioPath: Path.GetFullPath(translationAudio),
                responseFormat: AudioResponseFormat.Text);
            var response = await OpenAIClient.AudioEndpoint.CreateTranslationTextAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_02_02_Translation_Json()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var translationAudio = Path.GetFullPath("../../../Assets/Ja-botchan_1-1_1-2.mp3");
            using var request = new AudioTranslationRequest(
                audioPath: Path.GetFullPath(translationAudio),
                responseFormat: AudioResponseFormat.Json);
            var response = await OpenAIClient.AudioEndpoint.CreateTranslationJsonAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_02_03_Translation_VerboseJson()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var translationAudio = Path.GetFullPath("../../../Assets/Ja-botchan_1-1_1-2.mp3");
            using var request = new AudioTranslationRequest(
                audioPath: Path.GetFullPath(translationAudio),
                responseFormat: AudioResponseFormat.Verbose_Json);
            var response = await OpenAIClient.AudioEndpoint.CreateTranslationJsonAsync(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_03_01_Speech()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            var request = new SpeechRequest("Hello World!");
            async Task ChunkCallback(ReadOnlyMemory<byte> chunkCallback)
            {
                Assert.IsFalse(chunkCallback.IsEmpty);
                await Task.CompletedTask;
            }

            var response = await OpenAIClient.AudioEndpoint.CreateSpeechAsync(request, ChunkCallback);
            Assert.IsFalse(response.IsEmpty);
            await File.WriteAllBytesAsync("../../../Assets/HelloWorld.mp3", response.ToArray());
        }

        [Test]
        public async Task Test_03_02_SpeechWithInstructions()
        {
            Assert.IsNotNull(OpenAIClient.AudioEndpoint);
            const string instructions = "You are a computer program giving your first response to eager computer scientists. Slightly digitize your voice and make it sounds like the matrix.";
            var request = new SpeechRequest(
                input: "Hello World!",
                model: Model.TTS_GPT_4o_Mini,
                voice: Voice.Fable,
                instructions: instructions);
            async Task ChunkCallback(ReadOnlyMemory<byte> chunkCallback)
            {
                Assert.IsFalse(chunkCallback.IsEmpty);
                await Task.CompletedTask;
            }

            var response = await OpenAIClient.AudioEndpoint.CreateSpeechAsync(request, ChunkCallback);
            Assert.IsFalse(response.IsEmpty);
            await File.WriteAllBytesAsync("../../../Assets/HelloWorld_Instructions.mp3", response.ToArray());
        }
    }
}
