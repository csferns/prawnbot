﻿using Autofac;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IBotService
    {
        Task<ResponseBase> ConnectAsync(string token, IContainer autofacContainer);
        Task<ResponseBase> DisconnectAsync(bool switchBot = false);
        Task<ResponseBase> ReconnectAsync();
        Task<Response<object>> GetStatusAsync();
        Task<ResponseBase> SetBotRegionAsync(string regionName);
        ResponseBase ShutdownQuartz();
    }

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

        public async Task<ResponseBase> DisconnectAsync(bool switchBot = false)
        {
            await botBL.DisconnectAsync(switchBot);
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

        public ResponseBase ShutdownQuartz()
        {
            botBL.ShutdownQuartz();
            return new ResponseBase();
        }
    }
}
