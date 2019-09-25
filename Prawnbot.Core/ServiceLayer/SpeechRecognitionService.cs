using Prawnbot.Core.BusinessLayer;
using Prawnbot.Infrastructure;

namespace Prawnbot.Core.ServiceLayer
{
    public interface ISpeechRecognitionService
    {
        ResponseBase Setup();
    }

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
