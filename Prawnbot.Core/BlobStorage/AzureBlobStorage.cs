using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Prawnbot.Common.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.BlobStorage
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly IConfigUtility configUtility;

        public AzureBlobStorage(IConfigUtility configUtility)
        {
            this.configUtility = configUtility;
        }

        private CloudBlobContainer GetContainer(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configUtility.BlobStoreConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName.ToLower());
        }

        private async Task<CloudBlockBlob> GetCloudBlockBlobAsync(string containerName, string fileName)
        {
            CloudBlobContainer container = GetContainer(containerName);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob;
        }

        private CloudBlockBlob GetCloudBlockBlob(string containerName, string fileName)
        {
            CloudBlobContainer container = GetContainer(containerName);
            Task.Run(async () => await container.CreateIfNotExistsAsync()).ConfigureAwait(false);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob;
        }

        public MemoryStream Download(BlobContainerEnum container, string key)
        {
            CloudBlockBlob blob = GetCloudBlockBlob(container.ToString(), key);

            MemoryStream target = new MemoryStream();
            Task.Run(async () => await blob.DownloadToStreamAsync(target)).ConfigureAwait(false);

            return target;
        }

        public async Task<MemoryStream> DownloadAsync(BlobContainerEnum container, string key)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(container.ToString(), key);

            MemoryStream target = new MemoryStream();
            await blob.DownloadToStreamAsync(target).ConfigureAwait(false);

            return target;
        }

        public bool Exists(BlobContainerEnum container, string key)
        {
            CloudBlockBlob blob = GetCloudBlockBlob(container.ToString(), key);
            return blob.ExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<bool> ExistsAsync(BlobContainerEnum container, string key)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(container.ToString(), key);
            return await blob.ExistsAsync();
        }

        public Uri Upload(BlobContainerEnum container, byte[] file, string key)
        {
            CloudBlockBlob blob = GetCloudBlockBlob(container.ToString(), key);
            Task.Run(async () => await blob.UploadFromByteArrayAsync(file, 1, 1));

            return blob.Uri;
        }

        public Uri Upload(BlobContainerEnum container, MemoryStream file, string key)
        {
            CloudBlockBlob blob = GetCloudBlockBlob(container.ToString(), key);
            Task.Run(async () => await blob.UploadFromStreamAsync(file));

            return blob.Uri;
        }

        public async Task<Uri> UploadAsync(BlobContainerEnum container, byte[] file, string key)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(container.ToString(), key);
            await blob.UploadFromByteArrayAsync(file, 1, 1);

            return blob.Uri;
        }

        public async Task<Uri> UploadAsync(BlobContainerEnum container, MemoryStream file, string key)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(container.ToString(), key);
            await blob.UploadFromStreamAsync(file);

            return blob.Uri;
        }
    }
}
