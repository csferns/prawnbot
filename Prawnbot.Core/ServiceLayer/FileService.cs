using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Models;
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
        Response<string> WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName);
        Task<Response<bool>> WriteToFile(string valueToWrite, string fileName);
        Task<ListResponse<CSVColumns>> CreateCSVList(ulong id);
        Response<Dictionary<string,string>> GetAllConfigurationValues();
        ResponseBase SetConfigurationValue(string configurationName, string newConfigurationValue);
        ResponseBase SetEventListeners(bool newValue);
    }

    public class FileService : BaseService, IFileService
    {
        protected IFileBl _fileBl;

        public FileService()
        {
            _fileBl = new FileBl();
        }

        public async Task<Response<Uri>> GetUriFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _fileBl.GetUriFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> GetStreamFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _fileBl.GetStreamFromBlobStore(fileName, containerName));
        }

        public async Task<Response<Stream>> DownloadFileFromBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _fileBl.DownloadFileFromBlobStore(fileName, containerName));
        }

        public async Task<Response<bool>> UploadFileToBlobStore(string fileName, string containerName)
        {
            return LoadResponse(await _fileBl.UploadFileToBlobStore(fileName, containerName));
        }

        public async Task<Response<string[]>> ReadFromFileAsync(string fileName)
        {
            return LoadResponse(await _fileBl.ReadFromFileAsync(fileName));
        }

        public Response<string> WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName)
        {
            return LoadResponse(_fileBl.WriteToCSV(columns, id, guildName));
        }

        public async Task<Response<bool>> WriteToFile(string valueToWrite, string fileName)
        {
            return LoadResponse(await _fileBl.WriteToFile(valueToWrite, fileName));
        }

        public async Task<ListResponse<CSVColumns>> CreateCSVList(ulong id)
        {
            return LoadListResponse(await _fileBl.CreateCSVList(id));
        }

        public Response<Dictionary<string, string>> GetAllConfigurationValues()
        {
            return LoadResponse(_fileBl.GetAllConfigurationValues());
        }

        public ResponseBase SetConfigurationValue(string configurationName, string newConfigurationValue)
        {
            _fileBl.SetConfigurationValue(configurationName, newConfigurationValue);
            return new ResponseBase();
        }

        public ResponseBase SetEventListeners(bool newValue)
        {
            _fileBl.SetEventListeners(newValue);

            return new ResponseBase();
        }
    }
}
