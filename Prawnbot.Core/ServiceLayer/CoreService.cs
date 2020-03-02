using Discord;
using Discord.WebSocket;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public class CoreService : BaseService, ICoreService
    {
        private readonly ICoreBL coreBL;
        public CoreService(ICoreBL coreBL)
        {
            this.coreBL = coreBL;
        }

        public async Task<ResponseBase> MessageEventListeners(SocketUserMessage message)
        {
            await coreBL.MessageEventListeners(message);
            return new ResponseBase();
        }

        public Response<Process> CreateFfmpegProcess(string path)
        {
            return LoadResponse(coreBL.CreateFfmpegProcess(path));
        }

        public Response<SocketTextChannel> FindDefaultChannel(SocketGuild guild)
        {
            return LoadResponse(coreBL.FindDefaultChannel(guild));
        }

        public Response<string> FindEmojis(SocketUserMessage message)
        {
            return LoadResponse(coreBL.FindEmojis(message));
        }

        public ListResponse<SocketTextChannel> FindGuildTextChannels(SocketGuild guild)
        {
            return LoadListResponse(coreBL.FindGuildTextChannels(guild));
        }

        public Response<SocketTextChannel> FindTextChannel(ulong id)
        {
            return LoadResponse(coreBL.FindTextChannel(id));
        }

        public Response<SocketTextChannel> FindTextChannel(SocketGuild guild, ulong id)
        {
            return LoadResponse(coreBL.FindTextChannel(guild, id));
        }

        public Response<SocketTextChannel> FindTextChannel(SocketGuild guild, string channelName)
        {
            return LoadResponse(coreBL.FindTextChannel(guild, channelName));
        }

        public Response<SocketTextChannel> FindTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            return LoadResponse(coreBL.FindTextChannel(guild, channel));
        }

        public Response<string> FlipACoin(string headsValue, string tailsValue)
        {
            return LoadResponse(coreBL.FlipACoin(headsValue, tailsValue));
        }

        public ListResponse<SocketGuild> GetAllGuilds()
        {
            return LoadListResponse(coreBL.GetAllGuilds());
        }

        public async Task<ListResponse<IMessage>> GetAllMessagesAsync(ulong id, int limit = 50000)
        {
            return LoadListResponse(await coreBL.GetAllMessagesAsync(id, limit));
        }

        public ListResponse<SocketGuildUser> GetAllUsers()
        {
            return LoadListResponse(coreBL.GetAllUsers());
        }

        public Response<SocketGuild> GetGuild(string guildName)
        {
            return LoadResponse(coreBL.GetGuild(guildName));
        }

        public Response<SocketGuildUser> GetUser(string username)
        {
            return LoadResponse(coreBL.GetUser(username));
        }

        public async Task<ResponseBase> LeaveAudioAsync(IGuild guild)
        {
            await coreBL.LeaveAudioAsync(guild);
            return new ResponseBase();
        }

        public async Task<Response<IPStatus>> PingHostAsync(string nameOrAddress)
        {
            return LoadResponse(await coreBL.PingHostAsync(nameOrAddress));
        }

        public async Task<ResponseBase> SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText)
        {
            await coreBL.SendChannelMessageAsync(guild, channel, messageText);
            return new ResponseBase();
        }

        public async Task<ResponseBase> SendDMAsync(SocketGuildUser user, string messageText)
        {
            await coreBL.SendDMAsync(user, messageText);
            return new ResponseBase();
        }

        public async Task<ResponseBase> SendImageFromBlobStore(string fileName)
        {
            await coreBL.SendImageFromBlobStoreAsync(fileName);
            return new ResponseBase();
        }

        public async Task<ResponseBase> SetBotStatusAsync(UserStatus status)
        {
            await coreBL.SetBotStatusAsync(status);
            return new ResponseBase();
        }

        public Response<string> TagUser(ulong id)
        {
            return LoadResponse(coreBL.TagUser(id));
        }

        public async Task<ResponseBase> UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl)
        {
            await coreBL.UpdateRichPresenceAsync(name, activityType, streamUrl);
            return new ResponseBase();
        }

        public async Task<ListResponse<string>> YottaPrepend()
        {
            return LoadListResponse(await coreBL.YottaPrependAsync());
        }

        public async Task<Response<IMessage>> GetRandomQuoteAsync(ulong id)
        {
            return LoadResponse(await coreBL.GetRandomQuoteAsync(id));
        }

        public async Task<ResponseBase> BackupServerAsync(ulong id, bool server)
        {
            await coreBL.BackupServerAsync(id, server);

            return new ResponseBase();
        }

        public async Task<Response<GuildEmote>> GetEmoteFromGuild(ulong id, SocketGuild guild)
        {
            return LoadResponse(await coreBL.GetEmoteFromGuildAsync(id, guild));
        }

        public async Task<ListResponse<IMessage>> GetAllMessagesByTimestampAsync(ulong guildId, DateTime timestamp)
        {
            return LoadListResponse(await coreBL.GetAllMessagesByTimestampAsync(guildId, timestamp));
        }

        public Response<SocketGuild> GetGuildById(ulong guildId)
        {
            return LoadResponse(coreBL.GetGuildById(guildId));
        }

        public async Task<ResponseBase> CommandsAsync(bool includeNotImplemented)
        {
            await coreBL.CommandsAsync(includeNotImplemented);
            return new ResponseBase();
        }

        public async Task<ResponseBase> GetBotInfoAsync()
        {
            await coreBL.GetBotInfoAsync();
            return new ResponseBase();
        }

        public async Task<ResponseBase> StatusAsync()
        {
            await coreBL.StatusAsync();
            return new ResponseBase();
        }

        public ListResponse<SocketGuildUser> GetUsersByGuildId(ulong guildId)
        {
            var guild = GetGuildById(guildId);

            return LoadListResponse(guild.Entity.Users);
        }
    }
}
