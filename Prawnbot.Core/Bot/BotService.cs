using Discord;
using Discord.WebSocket;
using Prawnbot.Core.Base;
using Prawnbot.Core.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prawnbot.Core.Bot
{
    public interface IBotService
    {
        Task<Response<bool>> ConnectAsync(string token);
        Task<Response<bool>> DisconnectAsync(bool switchBot);
        Task<Response<bool>> ReconnectAsync();
        Task<Response<bool>> SetBotStatusAsync(UserStatus status);
        ListResponse<SocketGuildUser> GetAllUsers();
        Task<ListResponse<IMessage>> GetAllMessages(ulong id);
        ListResponse<SocketGuild> GetAllGuilds();
        Response<SocketTextChannel> GetDefaultChannel(SocketGuild guild);
        ListResponse<SocketTextChannel> GetGuildTextChannels(SocketGuild guild);
        Response<SocketTextChannel> GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel);
        Response<Process> CreateFfmpegProcess(string path);
    }

    public class BotService : BaseService, IBotService
    {
        protected IBotBl _businessLayer;

        public BotService()
        {
            _businessLayer = new BotBl();
        }

        public async Task<Response<bool>> ConnectAsync(string token)
        {
            return LoadResponse(await _businessLayer.ConnectAsync(token));
        }

        public async Task<Response<bool>> DisconnectAsync(bool switchBot)
        {
            return LoadResponse(await _businessLayer.DisconnectAsync(switchBot));
        }

        public ListResponse<SocketGuildUser> GetAllUsers()
        {
            return LoadListResponse(_businessLayer.GetAllUsers());
        }

        public async Task<Response<bool>> ReconnectAsync()
        {
            return LoadResponse(await _businessLayer.ReconnectAsync());
        }

        public async Task<Response<bool>> SetBotStatusAsync(UserStatus status)
        {
            return LoadResponse(await _businessLayer.SetBotStatusAsync(status));
        }

        public Response<Process> CreateFfmpegProcess(string path)
        {
            return LoadResponse(_businessLayer.CreateFfmpegProcess(path));
        }

        public async Task<ListResponse<IMessage>> GetAllMessages(ulong id)
        {
            return LoadListResponse(await _businessLayer.GetAllMessages(id));
        }

        public ListResponse<SocketGuild> GetAllGuilds()
        {
            throw new System.NotImplementedException();
        }

        public Response<SocketTextChannel> GetDefaultChannel(SocketGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public ListResponse<SocketTextChannel> GetGuildTextChannels(SocketGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public Response<SocketTextChannel> GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            throw new System.NotImplementedException();
        }
    }
}
