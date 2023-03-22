using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_09_FineTuning : AbstractTestFixture
    {
        private async Task<FileData> CreateTestTrainingDataAsync()
        {
            var lines = new List<string>
            {
                new FineTuningTrainingData("Company: BHFF insurance\\nProduct: allround insurance\\nAd:One stop shop for all your insurance needs!\\nSupported:", "yes"),
                new FineTuningTrainingData("Company: Loft conversion specialists\\nProduct: -\\nAd:Straight teeth in weeks!\\nSupported:", "no")
            };

            const string localTrainingDataPath = "fineTunesTestTrainingData.jsonl";
            await File.WriteAllLinesAsync(localTrainingDataPath, lines);

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
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await OpenAIClient.FineTuningEndpoint.CreateFineTuneJobAsync(request);

            Assert.IsNotNull(fineTuneResponse);
            var result = await OpenAIClient.FilesEndpoint.DeleteFileAsync(fileData);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_02_ListFineTuneJobs()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fineTuneJobs = await OpenAIClient.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
            }
        }

        [Test]
        public async Task Test_03_RetrieveFineTuneJobInfo()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fineTuneJobs = await OpenAIClient.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                var request = await OpenAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(job);
                Assert.IsNotNull(request);
                Console.WriteLine($"{request.Id} -> {request.Status}");
            }
        }

        [Test]
        public async Task Test_04_ListFineTuneEvents()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fineTuneJobs = await OpenAIClient.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "cancelled")
                {
                    continue;
                }

                var fineTuneEvents = await OpenAIClient.FineTuningEndpoint.ListFineTuneEventsAsync(job);
                Assert.IsNotNull(fineTuneEvents);
                Assert.IsNotEmpty(fineTuneEvents);

                Console.WriteLine($"{job.Id} -> status: {job.Status} | event count: {fineTuneEvents.Count}");

                foreach (var @event in fineTuneEvents)
                {
                    Console.WriteLine($"  {@event.CreatedAt} [{@event.Level}] {@event.Message}");
                }

                Console.WriteLine("");
            }
        }

        [Test]
        public async Task Test_05_CancelFineTuneJob()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fineTuneJobs = await OpenAIClient.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "pending")
                {
                    var result = await OpenAIClient.FineTuningEndpoint.CancelFineTuneJobAsync(job);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{job.Id} -> cancelled");
                }
            }
        }

        [Test]
        public async Task Test_06_StreamFineTuneEvents()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);
            var fileData = await CreateTestTrainingDataAsync();
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await OpenAIClient.FineTuningEndpoint.CreateFineTuneJobAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await OpenAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await OpenAIClient.FineTuningEndpoint.StreamFineTuneEventsAsync(fineTuneJob, fineTuneEvent =>
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }, cancellationTokenSource.Token);

            var jobInfo = await OpenAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Console.WriteLine($"{jobInfo.Id} -> {jobInfo.Status}");
            Assert.IsTrue(jobInfo.Status == "cancelled");
            var result = await OpenAIClient.FilesEndpoint.DeleteFileAsync(fileData, CancellationToken.None);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_07_StreamFineTuneEventsEnumerable()
        {
            Assert.IsNotNull(OpenAIClient.FineTuningEndpoint);

            var fileData = await CreateTestTrainingDataAsync();
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await OpenAIClient.FineTuningEndpoint.CreateFineTuneJobAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await OpenAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await foreach (var fineTuneEvent in OpenAIClient.FineTuningEndpoint.StreamFineTuneEventsEnumerableAsync(fineTuneJob, cancellationTokenSource.Token))
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }

            var jobInfo = await OpenAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Console.WriteLine($"{jobInfo.Id} -> {jobInfo.Status}");
            Assert.IsTrue(jobInfo.Status == "cancelled");
            var result = await OpenAIClient.FilesEndpoint.DeleteFileAsync(fileData, CancellationToken.None);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_08_DeleteFineTunedModel()
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
                Console.WriteLine("Your account does not have permissions to delete models.");
            }
        }
    }
}
