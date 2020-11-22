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

        public Response<FileStream> WriteToCSV(HashSet<CSVColumns> columns, string fileName)
        {
            return LoadResponse(fileBL.WriteToCSV(columns, fileName));
        }

        public async Task<ResponseBase> WriteToFileAsync(string valueToWrite, string fileName)
        {
            await fileBL.WriteToFileAsync(valueToWrite, fileName);
            return new ResponseBase();
        }

        public ListResponse<CSVColumns> CreateCSVList(HashSet<IMessage> messagesToAdd)
        {
            return LoadListResponse(fileBL.CreateCSVList(messagesToAdd));
        }
    }
}
