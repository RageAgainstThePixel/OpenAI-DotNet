using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_09_FineTuning
    {
        private async Task<FileData> CreateTestTrainingDataAsync(OpenAIClient api)
        {
            var lines = new List<string>
            {
                new FineTuningTrainingData("Company: BHFF insurance\\nProduct: allround insurance\\nAd:One stop shop for all your insurance needs!\\nSupported:", "yes"),
                new FineTuningTrainingData("Company: Loft conversion specialists\\nProduct: -\\nAd:Straight teeth in weeks!\\nSupported:", "no")
            };

            const string localTrainingDataPath = "fineTunesTestTrainingData.jsonl";
            await File.WriteAllLinesAsync(localTrainingDataPath, lines);

            var fileData = await api.FilesEndpoint.UploadFileAsync(localTrainingDataPath, "fine-tune");
            File.Delete(localTrainingDataPath);
            Assert.IsFalse(File.Exists(localTrainingDataPath));
            return fileData;
        }

        [Test]
        public async Task Test_01_CreateFineTuneJob()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fileData = await CreateTestTrainingDataAsync(api);
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await api.FineTuningEndpoint.CreateFineTuneJobAsync(request);

            Assert.IsNotNull(fineTuneResponse);
            var result = await api.FilesEndpoint.DeleteFileAsync(fileData);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_02_ListFineTuneJobs()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();
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
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                var request = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(job);
                Assert.IsNotNull(request);
                Console.WriteLine($"{request.Id} -> {request.Status}");
            }
        }

        [Test]
        public async Task Test_04_ListFineTuneEvents()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "cancelled")
                {
                    continue;
                }

                var fineTuneEvents = await api.FineTuningEndpoint.ListFineTuneEventsAsync(job);
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
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "pending")
                {
                    var result = await api.FineTuningEndpoint.CancelFineTuneJobAsync(job);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{job.Id} -> cancelled");
                }
            }
        }

        [Test]
        public async Task Test_06_StreamFineTuneEvents()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fileData = await CreateTestTrainingDataAsync(api);
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await api.FineTuningEndpoint.CreateFineTuneJobAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await api.FineTuningEndpoint.StreamFineTuneEventsAsync(fineTuneJob, fineTuneEvent =>
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }, cancellationTokenSource.Token);

            var jobInfo = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Console.WriteLine($"{jobInfo.Id} -> {jobInfo.Status}");
            Assert.IsTrue(jobInfo.Status == "cancelled");
            var result = await api.FilesEndpoint.DeleteFileAsync(fileData, CancellationToken.None);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_07_StreamFineTuneEventsEnumerable()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTuningEndpoint);

            var fileData = await CreateTestTrainingDataAsync(api);
            var request = new CreateFineTuneJobRequest(fileData);
            var fineTuneResponse = await api.FineTuningEndpoint.CreateFineTuneJobAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await foreach (var fineTuneEvent in api.FineTuningEndpoint.StreamFineTuneEventsEnumerableAsync(fineTuneJob, cancellationTokenSource.Token))
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }

            var jobInfo = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Console.WriteLine($"{jobInfo.Id} -> {jobInfo.Status}");
            Assert.IsTrue(jobInfo.Status == "cancelled");
            var result = await api.FilesEndpoint.DeleteFileAsync(fileData, CancellationToken.None);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Test_08_DeleteFineTunedModel()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ModelsEndpoint);

            var models = await api.ModelsEndpoint.GetModelsAsync();
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
                    var result = await api.ModelsEndpoint.DeleteFineTuneModelAsync(model);
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
