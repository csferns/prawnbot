using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public class ConsoleService : BaseService, IConsoleService
    {
        public IConsoleBL consoleBL;

        public ConsoleService(IConsoleBL consoleBL)
        {
            this.consoleBL = consoleBL;
        }

        public Response<bool> ValidCommand(string command)
        {
            return LoadResponse(consoleBL.ValidCommand(command));
        }

        public async Task<Response<bool>> HandleConsoleCommand(string command)
        {
            return LoadResponse(await consoleBL.HandleConsoleCommand(command));
        }
    }
}
