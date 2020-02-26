using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;

namespace Prawnbot.Core.ServiceLayer
{
    public class SpeechRecognitionService : BaseService, ISpeechRecognitionService
    {
        private readonly ISpeechRecognitionBL speechRecognitionBL;
        public SpeechRecognitionService(ISpeechRecognitionBL speechRecognitionBL)
        {
            this.speechRecognitionBL = speechRecognitionBL;
        }

        public ResponseBase Setup()
        {
            //speechRecognitionBL.Setup();

            return new ResponseBase();
        }
    }
}
