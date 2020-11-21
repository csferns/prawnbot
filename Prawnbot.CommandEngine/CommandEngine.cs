using Microsoft.Extensions.Logging;
using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class CommandEngine : ICommandEngine
    {
        private readonly ICommandParser commandParser;
        private readonly ICommandProcessor commandProcessor;
        private readonly ILogger<CommandEngine> logger;

        public CommandEngine(ICommandParser commandParser, ICommandProcessor commandProcessor, ILogger<CommandEngine> logger)
        {
            this.commandParser = commandParser;
            this.commandProcessor = commandProcessor;
            this.logger = logger;
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
                logger.LogError(e, "An error occured in the command engine: {0}", e.Message);
            }
        }
    }
}
