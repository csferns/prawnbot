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
        Response<FileStream> WriteToCSV(IList<CSVColumns> columns, string fileName);
        Task<ResponseBase> WriteToFileAsync(string valueToWrite, string fileName);
        ListResponse<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
    }
}
