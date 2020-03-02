using Prawnbot.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling.Interfaces
{
    public interface IAzureStorageService
    {
        Task<Response<Uri>> GetUriFromBlobStoreAsync(string fileName, string containerName);
        Task<Response<Stream>> GetStreamFromBlobStoreAsync(string fileName, string containerName);
        Task<Response<Stream>> DownloadFileFromBlobStoreAsync(string fileName, string containerName);
        Task<ResponseBase> UploadFileToBlobStoreAsync(string fileName, string containerName);
    }
}
