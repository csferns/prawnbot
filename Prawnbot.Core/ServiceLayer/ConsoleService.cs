using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IConsoleService
    {
        Response<bool> ValidCommand(string command);
        Task<ResponseBase> HandleConsoleCommand(string command);
        ResponseBase ConsoleCommands(string command);
    }

    public class ConsoleService : BaseService, IConsoleService
    {
        public IConsoleBl _consoleBl;

        public ConsoleService()
        {
            _consoleBl = new ConsoleBl();
        }

        public Response<bool> ValidCommand(string command)
        {
            return LoadResponse(_consoleBl.ValidCommand(command));
        }

        public async Task<ResponseBase> HandleConsoleCommand(string command)
        {
            await _consoleBl.HandleConsoleCommand(command);

            return new ResponseBase();
        }

        public ResponseBase ConsoleCommands(string command)
        {
            _consoleBl.ConsoleCommands(command);

            return new ResponseBase();
        }
    }
}
