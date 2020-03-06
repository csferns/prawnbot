using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Core.Custom.Collections;

namespace Prawnbot.CommandEngine
{
    public class Command : ICommand
    {
        public Command()
        {
            CommandComponents = new Bunch<string>();
        }

        public string CommandText { get; set; }
        public bool Valid { get; set; }
        public CommandsEnum ParsedCommand { get; set; }

        public IBunch<string> CommandComponents { get; set; }

        public int RequiredParameterCount { get; set; }
        public int? OptionalParameterCount { get; set; }

        public int TotalParameterCount => RequiredParameterCount + (OptionalParameterCount ?? 0);

        public bool HasCorrectParameterCount => CommandComponents.Count == RequiredParameterCount || CommandComponents.Count == TotalParameterCount;
    }
}
