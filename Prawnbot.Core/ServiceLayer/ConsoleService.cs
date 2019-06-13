using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IConsoleService
    {
        Response<bool> ValidCommand(string command);
        Task<Response<bool>> HandleConsoleCommand(string command);
    }

    public class ConsoleService : BaseService, IConsoleService
    {
        public IConsoleBl _consoleBl;

        public ConsoleService(CancellationTokenSource workerCancellationTokenSource)
        {
            _consoleBl = new ConsoleBl(workerCancellationTokenSource);
        }

        public Response<bool> ValidCommand(string command)
        {
            return LoadResponse(_consoleBl.ValidCommand(command));
        }

        public async Task<Response<bool>> HandleConsoleCommand(string command)
        {
            return LoadResponse(await _consoleBl.HandleConsoleCommand(command));
        }
    }
}
