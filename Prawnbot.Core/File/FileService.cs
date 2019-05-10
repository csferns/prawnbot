using Prawnbot.Core.Base;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.File
{
    public interface IFileService
    {
        Task<Response<string>> GetUrlFromBlobStore(string fileName, string containerName);
        Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName);
        Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName);
        Task<Response<string>> WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id, string guildName);
        Task<ListResponse<CSVColumns>> CreateCSVList(ulong id);
    }

    public class FileService : BaseService, IFileService
    {
        protected IFileBl _businessLayer;

        public FileService()
        {
            _businessLayer = new FileBl();
        }

        public async Task<Response<string>> GetUrlFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.GetUrlFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.DownloadFileFromBlobStore(fileName, containerName));
        }

        public async Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.UploadFileToBlobStore(fileName, containerName));
        }

        public async Task<Response<string>> WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id, string guildName)
        {
            return LoadResponse(await _businessLayer.WriteToCSV(columns, folderPath, startTime, id, guildName));
        }

        public async Task<ListResponse<CSVColumns>> CreateCSVList(ulong id)
        {
            return LoadListResponse(await _businessLayer.CreateCSVList(id));
        }
    }
}
