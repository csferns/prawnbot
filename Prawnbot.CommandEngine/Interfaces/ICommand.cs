using Prawnbot.Core.Custom.Collections;

namespace Prawnbot.CommandEngine.Interfaces
{
    public interface ICommand
    {
        string CommandText { get; set; }
        bool Valid { get; set; }
        CommandsEnum ParsedCommand { get; set; }
        IBunch<string> CommandComponents { get; set; }

        int RequiredParameterCount { get; set; }
        int? OptionalParameterCount { get; set; }
        int TotalParameterCount { get; }
        bool HasCorrectParameterCount { get; }
    }
}
