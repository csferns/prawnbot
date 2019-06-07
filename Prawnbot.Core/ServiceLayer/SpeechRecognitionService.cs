using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;

namespace Prawnbot.Core.ServiceLayer
{
    public interface ISpeechRecognitionService
    {
        ResponseBase Setup();
    }

    public class SpeechRecognitionService : BaseService, ISpeechRecognitionService
    {
        protected ISpeechRecognitionBl _speechRecognitionBl;
        public SpeechRecognitionService()
        {
            _speechRecognitionBl = new SpeechRecognitionBl();
        }

        public ResponseBase Setup()
        {
            _speechRecognitionBl.Setup();

            return new ResponseBase();
        }
    }
}
