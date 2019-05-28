using Prawnbot.Core.Base;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.LocalFileAccess
{
    public interface IFileService
    {
        Task<Response<Uri>> GetUriFromBlobStore(string fileName, string containerName);
        Task<Response<Stream>> GetStreamFromBlobStore(string fileName, string containerName);
        Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName);
        Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName);
        Response<string[]> ReadFromFile(string fileName);
        Response<string> WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName);
        Task<Response<bool>> WriteToFile(string valueToWrite, string fileName);
        Task<ListResponse<CSVColumns>> CreateCSVList(ulong id);
        Response<Dictionary<string,string>> GetAllConfigurationValues();
        ResponseBase SetConfigurationValue(string configurationName, string newConfigurationValue);
    }

    public class FileService : BaseService, IFileService
    {
        protected IFileBl _businessLayer;

        public FileService()
        {
            _businessLayer = new FileBl();
        }

        public async Task<Response<Uri>> GetUriFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.GetUriFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> GetStreamFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.GetStreamFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.DownloadFileFromBlobStore(fileName, containerName));
        }

        public async Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _businessLayer.UploadFileToBlobStore(fileName, containerName));
        }

        public Response<string[]> ReadFromFile(string fileName)
        {
            return LoadResponse(_businessLayer.ReadFromFile(fileName));
        }

        public Response<string> WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName)
        {
            return LoadResponse(_businessLayer.WriteToCSV(columns, id, guildName));
        }

        public async Task<Response<bool>> WriteToFile(string valueToWrite, string fileName)
        {
            return LoadResponse(await _businessLayer.WriteToFile(valueToWrite, fileName));
        }

        public async Task<ListResponse<CSVColumns>> CreateCSVList(ulong id)
        {
            return LoadListResponse(await _businessLayer.CreateCSVList(id));
        }

        public Response<Dictionary<string, string>> GetAllConfigurationValues()
        {
            return LoadResponse(_businessLayer.GetAllConfigurationValues());
        }

        public ResponseBase SetConfigurationValue(string configurationName, string newConfigurationValue)
        {
            _businessLayer.SetConfigurationValue(configurationName, newConfigurationValue);
            return new ResponseBase();
        }
    }
}
