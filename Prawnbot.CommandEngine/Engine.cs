using Prawnbot.CommandEngine.Interfaces;
using System;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class Engine : ICommandEngine
    {
        private readonly ICommandParser commandParser;
        private readonly ICommandProcessor commandProcessor;

        public Engine(ICommandParser commandParser, ICommandProcessor commandProcessor)
        {
            this.commandParser = commandParser;
            this.commandProcessor = commandProcessor;
        }

        public async Task BeginListen(Func<string> listenAction)
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
    }
}
