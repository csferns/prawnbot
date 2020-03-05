using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Logging;
using System;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class Engine : ICommandEngine
    {
        private readonly ICommandParser commandParser;
        private readonly ICommandProcessor commandProcessor;
        private readonly ILogging logging;

        public Engine(ICommandParser commandParser, ICommandProcessor commandProcessor, ILogging logging)
        {
            this.commandParser = commandParser;
            this.commandProcessor = commandProcessor;
            this.logging = logging;
        }

        public async Task BeginListen(Func<string> listenAction)
        {
            try
            {
                bool willContinue = true;

                while (willContinue)
                {
                    string commandText = listenAction.Invoke();

                    Command command = await commandParser.ParseCommand(commandText);

                    if (command.Valid)
                    {
                        willContinue = await commandProcessor.ProcessCommand(command);
                    }
                }
            }
            catch (Exception e)
            {
                logging.Log_Exception(e);
            }
        }
    }
}
