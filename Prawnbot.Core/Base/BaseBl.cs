using Prawnbot.Core.API;
using Prawnbot.Core.Bot;
using Prawnbot.Core.Command;
using Prawnbot.Core.DatabaseAccess;
using Prawnbot.Core.LocalFileAccess;
using Prawnbot.Core.Log;
using Prawnbot.Core.SpeechRecognition;
using Prawnbot.Core.Utility;

namespace Prawnbot.Core.Base
{
    public class BaseBl
    {
        protected static ILogging logging;

        public static ConfigUtility ConfigUtility;

        protected static IAPIBl _apiBl;
        protected static IAPIService _apiService;

        protected static IBotBl _botBl;
        protected static IBotService _botService;

        protected static ICommandBl _commandBl;
        protected static ICommandService _commandService;

        protected static IFileBl _fileBl;
        protected static IFileService _fileService;

        protected static IDatabaseAccessBl _dbAccessBl;
        protected static IDatabaseAccessService _dbAccessService;

        protected static ISpeechRecognitionBl _speechRecognitionBl;
        protected static ISpeechRecognitionService _speechRecognitionService;
    }
}
