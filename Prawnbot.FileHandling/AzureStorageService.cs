using Prawnbot.FileHandling.Interfaces;
using Prawnbot.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling
{
    public class AzureStorageService : BaseService, IAzureStorageService
    {
        private readonly IAzureStorageBL azureStorageBL;

        public AzureStorageService(IAzureStorageBL azureStorageBL)
        {
            this.azureStorageBL = azureStorageBL;
        }

        public async Task<Response<Uri>> GetUriFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await azureStorageBL.GetUriFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<Response<Stream>> GetStreamFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await azureStorageBL.GetStreamFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await azureStorageBL.DownloadFileFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<ResponseBase> UploadFileToBlobStoreAsync(string fileName, string containerName)
        {
            await azureStorageBL.UploadFileToBlobStoreAsync(fileName, containerName);
            return new ResponseBase();
        }
    }
}
