// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_11_VectorStores : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_VectorStores_SingleFile()
        {
            Assert.IsNotNull(OpenAIClient.VectorStoresEndpoint);

            const string testFilePath = "vector_file_test_1.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            FileResponse file = null;

            try
            {
                try
                {
                    file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, FilePurpose.Assistants);
                }
                finally
                {
                    if (File.Exists(testFilePath))
                    {
                        File.Delete(testFilePath);
                    }

                    Assert.IsFalse(File.Exists(testFilePath));
                }

                VectorStoreResponse vectorStore = null;

                try
                {
                    // create vector store
                    var createVectorStoreRequest = new CreateVectorStoreRequest("test-vector-store");
                    vectorStore = await OpenAIClient.VectorStoresEndpoint.CreateVectorStoreAsync(createVectorStoreRequest);
                    Assert.IsNotNull(vectorStore);
                    Assert.AreEqual("test-vector-store", vectorStore.Name);

                    // list vector stores
                    var vectorStores = await OpenAIClient.VectorStoresEndpoint.ListVectorStoresAsync();
                    Assert.IsNotNull(vectorStores);
                    Assert.IsNotEmpty(vectorStores.Items);

                    // modify vector store
                    IReadOnlyDictionary<string, object> metadata = new Dictionary<string, object> { { nameof(Test_01_VectorStores_SingleFile), DateTime.UtcNow } };
                    var modifiedVectorStore = await OpenAIClient.VectorStoresEndpoint.ModifyVectorStoreAsync(vectorStore, metadata: metadata);
                    Assert.IsNotNull(modifiedVectorStore);
                    Assert.AreEqual(vectorStore.Id, modifiedVectorStore.Id);

                    // retrieve vector store
                    var retrievedVectorStore = await OpenAIClient.VectorStoresEndpoint.GetVectorStoreAsync(vectorStore);
                    Assert.IsNotNull(retrievedVectorStore);
                    Assert.AreEqual(vectorStore.Id, retrievedVectorStore.Id);

                    VectorStoreFileResponse vectorStoreFile = null;

                    try
                    {
                        // create vector store file
                        vectorStoreFile = await OpenAIClient.VectorStoresEndpoint.CreateVectorStoreFileAsync(vectorStore, file, new ChunkingStrategy(ChunkingStrategyType.Static));
                        Assert.IsNotNull(vectorStoreFile);

                        // list vector store files
                        var vectorStoreFiles = await OpenAIClient.VectorStoresEndpoint.ListVectorStoreFilesAsync(vectorStore);
                        Assert.IsNotNull(vectorStoreFiles);
                        Assert.IsNotEmpty(vectorStoreFiles.Items);

                        // retrieve vector store file
                        var retrievedVectorStoreFile = await OpenAIClient.VectorStoresEndpoint.GetVectorStoreFileAsync(vectorStore, vectorStoreFile);
                        Assert.IsNotNull(retrievedVectorStoreFile);
                        Assert.AreEqual(vectorStoreFile.Id, retrievedVectorStoreFile.Id);
                    }
                    finally
                    {
                        if (vectorStoreFile != null)
                        {
                            // delete vector store file
                            var deletedVectorStoreFile = await OpenAIClient.VectorStoresEndpoint.DeleteVectorStoreFileAsync(vectorStore, vectorStoreFile);
                            Assert.IsNotNull(deletedVectorStoreFile);
                            Assert.IsTrue(deletedVectorStoreFile);
                        }
                    }
                }
                finally
                {
                    if (vectorStore != null)
                    {
                        // delete vector store
                        var deletedVectorStore = await OpenAIClient.VectorStoresEndpoint.DeleteVectorStoreAsync(vectorStore);
                        Assert.IsNotNull(deletedVectorStore);
                        Assert.IsTrue(deletedVectorStore);
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_02_VectorStores_BatchFiles()
        {
            Assert.IsNotNull(OpenAIClient.VectorStoresEndpoint);

            const string testFilePath1 = "vector_file_test_2_1.txt";
            const string testFilePath2 = "vector_file_test_2_2.txt";
            await File.WriteAllTextAsync(testFilePath1, "Knowledge is power!");
            await File.WriteAllTextAsync(testFilePath2, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath1));
            Assert.IsTrue(File.Exists(testFilePath2));
            ConcurrentBag<FileResponse> files = new();

            try
            {
                try
                {
                    var uploadTasks = new List<Task>
                    {
                        Task.Run(async () => files.Add(await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath1, FilePurpose.Assistants))),
                        Task.Run(async () => files.Add(await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath2, FilePurpose.Assistants)))
                    };

                    await Task.WhenAll(uploadTasks).ConfigureAwait(false);
                }
                finally
                {
                    if (File.Exists(testFilePath1))
                    {
                        File.Delete(testFilePath1);
                    }

                    if (File.Exists(testFilePath2))
                    {
                        File.Delete(testFilePath2);
                    }

                    Assert.IsFalse(File.Exists(testFilePath1));
                    Assert.IsFalse(File.Exists(testFilePath2));
                }

                VectorStoreResponse vectorStore = null;

                try
                {
                    var createVectorStoreRequest = new CreateVectorStoreRequest(name: "test-vector-store-batch", files.ToList());
                    vectorStore = await OpenAIClient.VectorStoresEndpoint.CreateVectorStoreAsync(createVectorStoreRequest);
                    Assert.IsNotNull(vectorStore);
                    Assert.AreEqual("test-vector-store-batch", vectorStore.Name);

                    // create vector store batch
                    var vectorStoreFileBatch = await OpenAIClient.VectorStoresEndpoint.CreateVectorStoreFileBatchAsync(vectorStore, files.ToList());
                    Assert.IsNotNull(vectorStoreFileBatch);

                    // cancel vector store batch
                    var cancelledVectorStoreFileBatch = await OpenAIClient.VectorStoresEndpoint.CancelVectorStoreFileBatchAsync(vectorStore, vectorStoreFileBatch);
                    Assert.IsNotNull(cancelledVectorStoreFileBatch);
                    Assert.IsTrue(cancelledVectorStoreFileBatch);

                    // create vector store batch
                    vectorStoreFileBatch = await OpenAIClient.VectorStoresEndpoint.CreateVectorStoreFileBatchAsync(vectorStore, files.ToList());
                    Assert.IsNotNull(vectorStoreFileBatch);

                    // currently no way to list vector store batches
                    //var vectorStoreFileBatches = await OpenAIClient.VectorStoresEndpoint.ListVectorStoreFileBatchesAsync(vectorStore);
                    //Assert.IsNotNull(vectorStoreFileBatches);
                    //Assert.IsNotEmpty(vectorStoreFileBatches.Items);

                    // retrieve vector store batch
                    var retrievedVectorStoreFileBatch = await vectorStoreFileBatch.WaitForStatusChangeAsync();
                    Assert.IsNotNull(retrievedVectorStoreFileBatch);
                    Assert.IsTrue(retrievedVectorStoreFileBatch.Status == VectorStoreFileStatus.Completed);

                    // list vector store batch files
                    var vectorStoreBatchFiles = await OpenAIClient.VectorStoresEndpoint.ListVectorStoreBatchFilesAsync(vectorStore, vectorStoreFileBatch);
                    Assert.IsNotNull(vectorStoreBatchFiles);
                    Assert.IsNotEmpty(vectorStoreBatchFiles.Items);

                    foreach (var file in vectorStoreBatchFiles.Items)
                    {
                        // get vector store batch file
                        var retrievedVectorStoreBatchFile = await OpenAIClient.VectorStoresEndpoint.GetVectorStoreFileAsync(vectorStore, file);
                        Assert.IsNotNull(retrievedVectorStoreBatchFile);
                        Assert.AreEqual(file.Id, retrievedVectorStoreBatchFile.Id);
                    }
                }
                finally
                {
                    if (vectorStore != null)
                    {
                        var deletedVectorStore = await OpenAIClient.VectorStoresEndpoint.DeleteVectorStoreAsync(vectorStore);
                        Assert.IsNotNull(deletedVectorStore);
                        Assert.IsTrue(deletedVectorStore);
                    }
                }
            }
            finally
            {
                if (!files.IsEmpty)
                {
                    var deleteTasks = files.Select(file => OpenAIClient.FilesEndpoint.DeleteFileAsync(file)).ToList();
                    await Task.WhenAll(deleteTasks).ConfigureAwait(false);
                    Assert.IsTrue(deleteTasks.TrueForAll(task => task.Result));
                }
            }
        }
    }
}
