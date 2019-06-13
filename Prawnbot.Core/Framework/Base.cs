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
        public static IAPIService _apiService;
        public static IBotService _botService;
        public static ICommandService _commandService;
        public static IFileService _fileService;
        public static IDatabaseAccessService _dbAccessService;
        public static ISpeechRecognitionService _speechRecognitionService;

        public static IAPIBl _apiBl;
        public static IBotBl _botBl;
        public static ICommandBl _commandBl;
        public static IFileBl _fileBl;
        public static IDatabaseAccessBl _databaseAccessBl;
        public static ISpeechRecognitionBl _speechRecognitionBl;
    }
}
