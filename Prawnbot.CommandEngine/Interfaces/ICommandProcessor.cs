using System.Threading.Tasks;

namespace Prawnbot.CommandEngine.Interfaces
{
    public interface ICommandProcessor
    {
        Task<bool> ProcessCommand(Command command);
    }
}
