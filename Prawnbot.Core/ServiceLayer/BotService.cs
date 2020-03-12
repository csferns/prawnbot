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

        public Response<object> GetStatus() 
        {
            return LoadResponse(botBL.GetStatus());
        }

        public async Task<ResponseBase> ConnectAsync(string token)
        {
            var response = await LoadResponseBaseAsync(() => botBL.ConnectAsync(token));
            return response;
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
