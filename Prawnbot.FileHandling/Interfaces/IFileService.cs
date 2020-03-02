using Discord;
using Prawnbot.Common.DTOs;
using Prawnbot.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling.Interfaces
{
    /// <summary>
    /// Contains all the methods to deal with writing data to files
    /// </summary>
    public interface IFileService
    {
        Task<ListResponse<string>> ReadFromFileAsync(string fileName);
        Response<FileStream> WriteToCSV(IList<CSVColumns> columns, string fileName);
        Task<ResponseBase> WriteToFileAsync(string valueToWrite, string fileName);
        ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
    }
}
