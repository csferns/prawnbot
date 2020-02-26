using Autofac;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public class BotService : BaseService, IBotService
    {
        private readonly IBotBL botBL;

        public BotService(IBotBL botBL)
        { 
            this.botBL = botBL;
        }

        public async Task<Response<object>> GetStatusAsync()
        {
            return LoadResponse(await botBL.GetStatusAsync());
        }

        public async Task<ResponseBase> ConnectAsync(string token, IContainer autofacContainer)
        {
            await botBL.ConnectAsync(token, autofacContainer);
            return new ResponseBase();
        }

        public async Task<ResponseBase> DisconnectAsync(bool shutdown = false)
        {
            await botBL.DisconnectAsync(shutdown);
            return new ResponseBase();
        }

        public async Task<ResponseBase> ReconnectAsync()
        {
            await botBL.ReconnectAsync();
            return new ResponseBase();
        }

        public async Task<ResponseBase> SetBotRegionAsync(string regionName)
        {
            await botBL.SetBotRegionAsync(regionName);
            return new ResponseBase();
        }
    }
}
