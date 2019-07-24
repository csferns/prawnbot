using Prawnbot.Core.BusinessLayer;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IBotService
    {
        Task<Response<bool>> ConnectAsync(string token);
        Task<Response<bool>> DisconnectAsync(bool switchBot);
        Task<Response<bool>> ReconnectAsync();
    }

    public class BotService : BaseService, IBotService
    {
        private readonly IBotBL botBL;

        public BotService(IBotBL botBL)
        { 
            this.botBL = botBL;
        }

        public async Task<Response<bool>> ConnectAsync(string token)
        {
            return LoadResponse(await botBL.ConnectAsync(token));
        }

        public async Task<Response<bool>> DisconnectAsync(bool switchBot)
        {
            return LoadResponse(await botBL.DisconnectAsync(switchBot));
        }

        public async Task<Response<bool>> ReconnectAsync()
        {
            return LoadResponse(await botBL.ReconnectAsync());
        }
    }
}
