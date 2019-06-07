using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Framework
{
    public class Base
    {
        public Base()
        {

        }

        // Services
        public IAPIService _apiService;
        public IBotService _botService;
        public ICommandService _commandService;
        public IFileService _fileService;
        public IDatabaseAccessService _dbAccessService;
        public ISpeechRecognitionService _speechRecognitionService;

        public IAPIBl _apiBl;
        public IBotBl _botBl;
        public ICommandBl _commandBl;
        public IFileBl _fileBl;
        public IDatabaseAccessBl _databaseAccessBl;
        public ISpeechRecognitionBl _speechRecognitionBl;
    }
}
