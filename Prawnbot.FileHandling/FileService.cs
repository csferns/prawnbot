using Discord;
using Prawnbot.Common.DTOs;
using Prawnbot.FileHandling.Interfaces;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling
{
    public class FileService : BaseService, IFileService
    {
        private readonly IFileBL fileBL;

        public FileService(IFileBL fileBL)
        {
            this.fileBL = fileBL;
        }

        public async Task<ListResponse<string>> ReadFromFileAsync(string fileName)
        {
            return LoadListResponse(await fileBL.ReadFromFileAsync(fileName));
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
