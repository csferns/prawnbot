using Discord;
using Discord.WebSocket;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface ICoreService
    {
        Task<ResponseBase> SendImageFromBlobStore(string fileName);
        Task<ResponseBase> MessageEventListeners(SocketUserMessage message);
        Task<ListResponse<string>> YottaPrepend();
        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        Task<ResponseBase> SetBotStatusAsync(UserStatus status);
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
        /// 
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        ListResponse<SocketGuildUser> GetUsersByGuildId(ulong guildId);

        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        Task<ListResponse<IMessage>> GetAllMessagesAsync(ulong textChannelId, int limit = 50000); 
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
        Response<string> TagUser(ulong id);
        Task<Response<IPStatus>> PingHostAsync(string nameOrAddress);
        Task<Response<IMessage>> GetRandomQuoteAsync(ulong id);
        Task<ResponseBase> BackupServerAsync(ulong id, bool server);
        Task<Response<GuildEmote>> GetEmoteFromGuild(ulong id, SocketGuild guild);
        Task<ListResponse<IMessage>> GetAllMessagesByTimestampAsync(ulong guildId, DateTime timestamp);
        Response<SocketGuild> GetGuildById(ulong guildId);

        Task<ResponseBase> CommandsAsync(bool includeNotImplemented);
        Task<ResponseBase> GetBotInfoAsync();
        Task<ResponseBase> StatusAsync();
        Task<ResponseBase> ChangeIconAsync(string imageUri);
        Task<ResponseBase> ChangeIconAsync(Uri imageUri = null);
    }
}
