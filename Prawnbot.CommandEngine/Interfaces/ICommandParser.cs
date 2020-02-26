using System.Threading.Tasks;

namespace Prawnbot.CommandEngine.Interfaces
{
    public interface ICommandParser
    {
        Task<Command> ParseCommand(string commandText);
    }
}
