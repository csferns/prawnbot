using Discord;
using Discord.WebSocket;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface ICoreService
    {
        Task<ResponseBase> SendImageFromBlobStore(string fileName);
        Task<Response<bool>> ContainsUser(SocketUserMessage message);
        Task<Response<bool>> ContainsText(SocketUserMessage message);
        Response<string> FlipACoin(string headsValue, string tailsValue);
        Task<Response<string[]>> YottaPrepend();
        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        Task<Response<bool>> SetBotStatusAsync(UserStatus status);
        /// <summary>
        /// Updates the bot's rich presence
        /// </summary>
        /// <param name="name">Name of the game</param>
        /// <param name="activityType">Activity type of the game</param>
        /// <param name="streamUrl">(Optional) stream url</param>
        /// <returns></returns>
        Task<ResponseBase> UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl);
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Response<SocketGuildUser> GetUser(string username);
        /// <summary>
        /// Get all the current users in the servers the bot is connected to
        /// </summary>
        /// <returns></returns>
        ListResponse<SocketGuildUser> GetAllUsers();
        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        Task<ListResponse<IMessage>> GetAllMessagesAsync(ulong id);
        /// <summary>
        /// Get a guild by name
        /// </summary>
        /// <param name="guildName">Name of the guild</param>
        /// <returns></returns>
        Response<SocketGuild> GetGuild(string guildName);
        /// <summary>
        /// Get all the current guilds the bot is connected to
        /// </summary>
        /// <returns>List of IGuilds</returns>
        ListResponse<SocketGuild> GetAllGuilds();
        /// <summary>
        /// Gets the default channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>SocketTextChannel</returns>
        Response<SocketTextChannel> FindDefaultChannel(SocketGuild guild);
        /// <summary>
        /// Gets a text channel with the supplied ID
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <returns></returns>
        Response<SocketTextChannel> FindTextChannel(ulong id);
        /// <summary>
        /// Gets a text channel with the supplied guild and id
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Response<SocketTextChannel> FindTextChannel(SocketGuild guild, ulong id);
        /// <summary>
        /// Gets a text channel with a supplied guild and name
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        Response<SocketTextChannel> FindTextChannel(SocketGuild guild, string channelName);
        /// <summary>
        /// Gets a channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <param name="channel">Channel</param>
        /// <returns>SocketTextChannel</returns>
        Response<SocketTextChannel> FindTextChannel(SocketGuild guild, SocketTextChannel channel);
        /// <summary>
        /// Gets all the channels of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>IReadOnlyCollection of SocketTextChannels</returns>
        ListResponse<SocketTextChannel> FindGuildTextChannels(SocketGuild guild);
        /// <summary>
        /// Create an FFMPEG process for the audio commands
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Response<Process> CreateFfmpegProcess(string path);
        /// <summary>
        /// Leaves the current audio channel
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        Task<ResponseBase> LeaveAudioAsync(IGuild guild);
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="guild">Guild to send it to</param>
        /// <param name="channel">Channel to send it to</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task<ResponseBase> SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText);
        Response<string> FindEmojis(SocketUserMessage message);
        /// <summary>
        /// Send a message through a DM
        /// </summary>
        /// <param name="user">User to DM</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task<ResponseBase> SendDMAsync(SocketGuildUser user, string messageText);
        Task<ResponseBase> ReactToMessageAsync(string Content, string[] lookupValues, string replyText, bool giffedMessage = false, string gifSearchText = null);
        Response<string> TagUser(ulong id);
        Response<bool> PingHost(string nameOrAddress);
        Task<Response<IMessage>> GetRandomQuoteAsync(ulong id);
        Task<ResponseBase> BackupServerAsync(ulong id, bool server);
        Task<Response<GuildEmote>> GetEmoteFromGuild(ulong id, SocketGuild guild);
    }

    public class CoreService : BaseService, ICoreService
    {
        private readonly ICoreBL coreBL;
        public CoreService(ICoreBL coreBL)
        {
            this.coreBL = coreBL;
        }

        public async Task<Response<bool>> ContainsText(SocketUserMessage message)
        {
            return LoadResponse(await coreBL.ContainsText(message));
        }

        public async Task<Response<bool>> ContainsUser(SocketUserMessage message)
        {
            return LoadResponse(await coreBL.ContainsUser(message));
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

        public async Task<ListResponse<IMessage>> GetAllMessagesAsync(ulong id)
        {
            return LoadListResponse<IMessage>(await coreBL.GetAllMessagesAsync(id));
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

        public Response<bool> PingHost(string nameOrAddress)
        {
            return LoadResponse(coreBL.PingHost(nameOrAddress));
        }

        public async Task<ResponseBase> ReactToMessageAsync(string Content, string[] lookupValues, string replyText, bool giffedMessage = false, string gifSearchText = null)
        {
            await coreBL.ReactToMessageAsync(Content, lookupValues, replyText, giffedMessage, gifSearchText);
            return new ResponseBase();
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
            await coreBL.SendImageFromBlobStore(fileName);
            return new ResponseBase();
        }

        public async Task<Response<bool>> SetBotStatusAsync(UserStatus status)
        {
            return LoadResponse(await coreBL.SetBotStatusAsync(status));
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

        public async Task<Response<string[]>> YottaPrepend()
        {
            return LoadResponse(await coreBL.YottaPrepend());
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
            return LoadResponse(await coreBL.GetEmoteFromGuild(id, guild));
        }
    }
}
