using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling.Interfaces
{
    public interface IAzureStorageBL
    {
        Task<CloudBlobContainer> GetBlobContainer(string containerName);
        Task<Uri> GetUriFromBlobStoreAsync(string fileName, string containerName);
        Task<Stream> GetStreamFromBlobStoreAsync(string fileName, string containerName);
        Task<Stream> DownloadFileFromBlobStoreAsync(string fileName, string containerName);
        Task UploadFileToBlobStoreAsync(string fileName, string containerName);
    }
}
