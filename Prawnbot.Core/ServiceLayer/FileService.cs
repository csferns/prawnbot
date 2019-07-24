using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IFileService
    {
        Task<Response<Uri>> GetUriFromBlobStore(string fileName, string containerName);
        Task<Response<Stream>> GetStreamFromBlobStore(string fileName, string containerName);
        Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName);
        Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName);
        Task<Response<string[]>> ReadFromFileAsync(string fileName);
        ResponseBase WriteToCSV(IList<CSVColumns> columns, ulong? id, string fileName);
        Task<Response<bool>> WriteToFile(string valueToWrite, string fileName);
        ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
    }

    public class FileService : BaseService, IFileService
    {
        private readonly IFileBL fileBL;

        public FileService(IFileBL fileBL)
        {
            this.fileBL = fileBL;
        }

        public async Task<Response<Uri>> GetUriFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.GetUriFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> GetStreamFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.GetStreamFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.DownloadFileFromBlobStore(fileName, containerName));
        }

        public async Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await fileBL.UploadFileToBlobStore(fileName, containerName));
        }

        public async Task<Response<string[]>> ReadFromFileAsync(string fileName)
        {
            return LoadResponse(await fileBL.ReadFromFileAsync(fileName));
        }

        public ResponseBase WriteToCSV(IList<CSVColumns> columns, ulong? id, string fileName)
        {
            fileBL.WriteToCSV(columns, id, fileName);
            return new ResponseBase();
        }

        public async Task<Response<bool>> WriteToFile(string valueToWrite, string fileName)
        {
            return LoadResponse(await fileBL.WriteToFile(valueToWrite, fileName));
        }

        public ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd)
        {
            return LoadListResponse(fileBL.CreateCSVList(messagesToAdd));
        }
    }
}
