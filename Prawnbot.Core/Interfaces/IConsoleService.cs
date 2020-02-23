using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IConsoleService
    {
        Response<bool> ValidCommand(string command);
        Task<Response<bool>> HandleConsoleCommand(string command);
    }
}
