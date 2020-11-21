using Discord;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public class FileService : BaseService, IFileService
    {
        private readonly IFileBL fileBL;

        public FileService(IFileBL fileBL)
        {
            this.fileBL = fileBL;
        }

        public async Task<Response<Uri>> GetUriFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.GetUriFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<Response<Stream>> GetStreamFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.GetStreamFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStoreAsync(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.DownloadFileFromBlobStoreAsync(fileName, containerName));
        }

        public async Task<ResponseBase> UploadFileToBlobStoreAsync(string fileName, string containerName)
        {
            await fileBL.UploadFileToBlobStoreAsync(fileName, containerName);
            return new ResponseBase();
        }

        public Response<FileStream> WriteToCSV(IList<CSVColumns> columns, string fileName)
        {
            return LoadResponse(fileBL.WriteToCSV(columns, fileName));
        }

        public async Task<ResponseBase> WriteToFileAsync(string valueToWrite, string fileName)
        {
            await fileBL.WriteToFileAsync(valueToWrite, fileName);
            return new ResponseBase();
        }

        public ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd)
        {
            return LoadListResponse(fileBL.CreateCSVList(messagesToAdd));
        }
    }
}
