using System;

namespace Prawnbot.Infrastructure
{
    public class ResponseBase
    {
        public void SetException(string userMessage, Exception exception)
        {
            this.UserMessage = userMessage;
            this.Exception = exception;
        }

        public string UserMessage { get; private set; }
        public Exception Exception { get; private set; }
        public bool Success
        {
            get { return Exception == null; }
        }
    }
}
