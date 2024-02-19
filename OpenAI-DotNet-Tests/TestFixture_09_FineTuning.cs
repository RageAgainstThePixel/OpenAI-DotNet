// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_09_FineTuning : AbstractTestFixture
    {
        private async Task<FileResponse> CreateTestTrainingDataAsync()
        {
            var conversations = new List<Conversation>
            {
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "What's the capital of France?"),
                    new(Role.Assistant, "Paris, as if everyone doesn't know that already.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "Who wrote 'Romeo and Juliet'?"),
                    new(Role.Assistant, "Oh, just some guy named William Shakespeare. Ever heard of him?")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "How far is the Moon from Earth?"),
                    new(Role.Assistant, "Around 384,400 kilometers. Give or take a few, like that really matters.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "What's the capital of France?"),
                    new(Role.Assistant, "Paris, as if everyone doesn't know that already.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "Who wrote 'Romeo and Juliet'?"),
                    new(Role.Assistant, "Oh, just some guy named William Shakespeare. Ever heard of him?")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "How far is the Moon from Earth?"),
                    new(Role.Assistant, "Around 384,400 kilometers. Give or take a few, like that really matters.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "What's the capital of France?"),
                    new(Role.Assistant, "Paris, as if everyone doesn't know that already.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "Who wrote 'Romeo and Juliet'?"),
                    new(Role.Assistant, "Oh, just some guy named William Shakespeare. Ever heard of him?")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "How far is the Moon from Earth?"),
                    new(Role.Assistant, "Around 384,400 kilometers. Give or take a few, like that really matters.")
                }),
                new(new List<Message>
                {
                    new(Role.System, "Marv is a factual chatbot that is also sarcastic."),
                    new(Role.User, "How far is the Moon from Earth?"),
                    new(Role.Assistant, "Around 384,400 kilometers. Give or take a few, like that really matters.")
                })
            };
            const string localTrainingDataPath = "fineTunesTestTrainingData.jsonl";
            await File.WriteAllLinesAsync(localTrainingDataPath, conversations.Select(conversation => conversation.ToString()));

            var fileData = await OpenAIClient.FilesEndpoint.UploadFileAsync(localTrainingDataPath, "fine-tune");
            File.Delete(localTrainingDataPath);
            Assert.IsFalse(File.Exists(localTrainingDataPath));
            return fileData;
        }

        [Test]
        public async Task Test_01_CreateFineTuneJob()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fileData = await CreateTestTrainingDataAsync();
            Assert.IsNotNull(fileData);
            var request = new CreateFineTuneJobRequest(Model.GPT3_5_Turbo, fileData);
            var job = await OpenAIClient.FineTuningEndpoint.CreateJobAsync(request);

            Assert.IsNotNull(job);
            Console.WriteLine($"Started {job.Id} | Status: {job.Status}");
        }

        [Test]
        public async Task Test_02_ListFineTuneJobs()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var jobList = await OpenAIClient.FineTuningEndpoint.ListJobsAsync();
            Assert.IsNotNull(jobList);
            Assert.IsNotEmpty(jobList.Items);

            foreach (var job in jobList.Items.OrderByDescending(job => job.CreatedAt))
            {
                Assert.IsNotNull(job);
                Assert.IsNotNull(job.Client);
                Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
            }
        }

        [Test]
        public async Task Test_03_RetrieveFineTuneJobInfo()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var jobList = await OpenAIClient.FineTuningEndpoint.ListJobsAsync();
            Assert.IsNotNull(jobList);
            Assert.IsNotEmpty(jobList.Items);

            foreach (var job in jobList.Items.OrderByDescending(job => job.CreatedAt))
            {
                var response = await OpenAIClient.FineTuningEndpoint.GetJobInfoAsync(job);
                Assert.IsNotNull(response);
                Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
            }
        }

        [Test]
        public async Task Test_04_ListFineTuneEvents()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var jobList = await OpenAIClient.FineTuningEndpoint.ListJobsAsync();
            Assert.IsNotNull(jobList);
            Assert.IsNotEmpty(jobList.Items);

            foreach (var job in jobList.Items)
            {
                if (job.Status == JobStatus.Cancelled) { continue; }

                var eventList = await OpenAIClient.FineTuningEndpoint.ListJobEventsAsync(job);
                Assert.IsNotNull(eventList);
                Assert.IsNotEmpty(eventList.Items);
                Console.WriteLine($"{job.Id} -> status: {job.Status} | event count: {eventList.Items.Count}");

                foreach (var @event in eventList.Items.OrderByDescending(@event => @event.CreatedAt))
                {
                    Assert.IsNotNull(@event);
                    Assert.IsNotNull(@event.Client);
                    Console.WriteLine($"  {@event.CreatedAt} [{@event.Level}] {@event.Message}");
                }

                Console.WriteLine("");
            }
        }

        [Test]
        public async Task Test_05_CancelFineTuneJob()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var jobList = await OpenAIClient.FineTuningEndpoint.ListJobsAsync();
            Assert.IsNotNull(jobList);
            Assert.IsNotEmpty(jobList.Items);

            foreach (var job in jobList.Items)
            {
                if (job.Status is > JobStatus.NotStarted and < JobStatus.Succeeded)
                {
                    var result = await OpenAIClient.FineTuningEndpoint.CancelJobAsync(job);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{job.Id} -> cancelled");
                    result = await OpenAIClient.FilesEndpoint.DeleteFileAsync(job.TrainingFile);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{job.TrainingFile} -> deleted");
                }
            }
        }

        [Test]
        public async Task Test_06_DeleteFineTunedModel()
        {
            Assert.IsNotNull(OpenAIClient.ModelsEndpoint);
            var models = await OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(models);
            Assert.IsNotEmpty(models);

            try
            {
                foreach (var model in models)
                {
                    if (model.OwnedBy.Contains("openai") ||
                        model.OwnedBy.Contains("system"))
                    {
                        continue;
                    }

                    Console.WriteLine(model);
                    var result = await OpenAIClient.ModelsEndpoint.DeleteFineTuneModelAsync(model);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{model.Id} -> deleted");
                    break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("You have insufficient permissions for this operation. You need to be this role: Owner.");
            }
        }
    }
}
