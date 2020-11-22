using System.Collections.Generic;

namespace Prawnbot.CommandEngine
{
    public class Command
    {
        public Command()
        {
            CommandComponents = new HashSet<string>();
        }

        public string CommandText { get; set; }
        public bool Valid { get; set; }
        public CommandsEnum ParsedCommand { get; set; }

        public ICollection<string> CommandComponents { get; set; }

        public int RequiredParameterCount { get; set; }
        public int? OptionalParameterCount { get; set; }

        public int TotalParameterCount => RequiredParameterCount + (OptionalParameterCount ?? 0);

        public bool HasCorrectParameterCount => CommandComponents.Count == RequiredParameterCount || CommandComponents.Count == TotalParameterCount;
    }
}
