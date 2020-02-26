using Discord;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    /// <summary>
    /// Contains all the methods to deal with writing data to files
    /// </summary>
    public interface IFileService
    {
        Task<Response<Uri>> GetUriFromBlobStoreAsync(string fileName, string containerName);
        Task<Response<Stream>> GetStreamFromBlobStoreAsync(string fileName, string containerName);
        Task<Response<Stream>> DownloadFileFromBlobStoreAsync(string fileName, string containerName);
        Task<ResponseBase> UploadFileToBlobStoreAsync(string fileName, string containerName);
        Task<ListResponse<string>> ReadFromFileAsync(string fileName);
        Response<FileStream> WriteToCSV(IList<CSVColumns> columns, string fileName);
        Task<ResponseBase> WriteToFileAsync(string valueToWrite, string fileName);
        ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
    }
}
