using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FileTunes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_07_FineTunes
    {
        private async Task<FileData> CreateTestTrainingDataAsync(OpenAIClient api)
        {

            var lines = new List<string>
            {
                new FineTunesTrainingData("Company: BHFF insurance\\nProduct: allround insurance\\nAd:One stop shop for all your insurance needs!\\nSupported:", "yes"),
                new FineTunesTrainingData("Company: Loft conversion specialists\\nProduct: -\\nAd:Straight teeth in weeks!\\nSupported:", "no")
            };

            const string localTrainingDataPath = "fineTunesTestTrainingData.jsonl";
            await File.WriteAllLinesAsync(localTrainingDataPath, lines);

            var fileData = api.FilesEndpoint.UploadFileAsync(localTrainingDataPath, "fine-tune").Result;
            File.Delete(localTrainingDataPath);
            Assert.IsFalse(File.Exists(localTrainingDataPath));
            return fileData;
        }

        [Test]
        public void Test_01_CreateFineTune()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fileData = CreateTestTrainingDataAsync(api).Result;
            var request = new CreateFineTuneRequest(fileData);
            var fineTuneResponse = api.FineTunesEndpoint.CreateFineTuneAsync(request).Result;

            Assert.IsNotNull(fineTuneResponse);
        }

        [Test]
        public void Test_02_ListFineTunes()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fineTuneJobs = api.FineTunesEndpoint.ListFineTuneJobsAsync().Result;
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
            }
        }

        [Test]
        public void Test_03_RetrieveFineTuneInfo()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fineTuneJobs = api.FineTunesEndpoint.ListFineTuneJobsAsync().Result;
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                var request = api.FineTunesEndpoint.RetrieveFineTuneJobInfoAsync(job).Result;
                Assert.IsNotNull(request);
                Console.WriteLine($"{request.Id} -> {request.Status}");
            }
        }

        [Test]
        public void Test_04_ListFineTuneEvents()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fineTuneJobs = api.FineTunesEndpoint.ListFineTuneJobsAsync().Result;
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "cancelled")
                {
                    continue;
                }

                var fineTuneEvents = api.FineTunesEndpoint.ListFineTuneEventsAsync(job).Result;
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
        public void Test_05_CancelFineTunes()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fineTuneJobs = api.FineTunesEndpoint.ListFineTuneJobsAsync().Result;
            Assert.IsNotNull(fineTuneJobs);
            Assert.IsNotEmpty(fineTuneJobs);

            foreach (var job in fineTuneJobs)
            {
                if (job.Status == "pending")
                {
                    var result = api.FineTunesEndpoint.CancelFineTuneJob(job).Result;
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
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fileData = await CreateTestTrainingDataAsync(api);
            var request = new CreateFineTuneRequest(fileData);
            var fineTuneResponse = await api.FineTunesEndpoint.CreateFineTuneAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await api.FineTunesEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await api.FineTunesEndpoint.StreamFineTuneEventsAsync(fineTuneJob, fineTuneEvent =>
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }, cancellationTokenSource.Token);

            var jobInfo = await api.FineTunesEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Assert.IsTrue(jobInfo.Status == "cancelled");
        }

        [Test]
        public async Task Test_07_StreamFineTuneEventsEnumerable()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.FineTunesEndpoint);

            var fileData = await CreateTestTrainingDataAsync(api);
            var request = new CreateFineTuneRequest(fileData);
            var fineTuneResponse = await api.FineTunesEndpoint.CreateFineTuneAsync(request);
            Assert.IsNotNull(fineTuneResponse);

            var fineTuneJob = await api.FineTunesEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneResponse);
            Assert.IsNotNull(fineTuneJob);
            Console.WriteLine($"{fineTuneJob.Id} ->");
            var cancellationTokenSource = new CancellationTokenSource();

            await foreach (var fineTuneEvent in api.FineTunesEndpoint.StreamFineTuneEventsEnumerableAsync(fineTuneJob, cancellationTokenSource.Token))
            {
                Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
                cancellationTokenSource.Cancel();
            }

            var jobInfo = await api.FineTunesEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
            Assert.IsNotNull(jobInfo);
            Console.WriteLine($"{jobInfo.Id} -> {jobInfo.Status}");
            Assert.IsTrue(jobInfo.Status == "cancelled");
        }

        [Test]
        public async Task Test_08_DeleteFineTunedModel()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ModelsEndpoint);

            var models = await api.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(models);
            Assert.IsNotEmpty(models);

            foreach (var model in models)
            {
                if (model.OwnedBy == api.OpenAIAuthentication.Organization)
                {
                    Console.WriteLine(model);
                    var result = await api.ModelsEndpoint.DeleteFineTuneModelAsync(model);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result);
                    Console.WriteLine($"{model.Id} -> deleted");
                }
            }
        }
    }
}
