using Discord;
using Discord.WebSocket;
using Prawnbot.Core.Collections;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface ICoreBL
    {
        Task SendImageFromBlobStoreAsync(string fileName);
        Task MessageEventListeners(SocketUserMessage message);
        Task<Bunch<string>> YottaPrependAsync();
        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        Task SetBotStatusAsync(UserStatus status = UserStatus.Online);
        /// <summary>
        /// Updates the bot's rich presence
        /// </summary>
        /// <param name="name">Name of the game</param>
        /// <param name="activityType">Activity type of the game</param>
        /// <param name="streamUrl">(Optional) stream url</param>
        /// <returns></returns>
        Task UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl);
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        SocketGuildUser GetUser(string username);
        /// <summary>
        /// Get all the current users in the servers the bot is connected to
        /// </summary>
        /// <returns></returns>
        Bunch<SocketGuildUser> GetAllUsers();
        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        Task<Bunch<IMessage>> GetAllMessagesAsync(ulong id, int limit = 50000);
        Task<Bunch<IMessage>> GetAllMessagesByTimestampAsync(ulong guildId, DateTime timestamp);
        Task<Bunch<IMessage>> GetUserMessagesAsync(ulong id, int limit = 50000);
        /// <summary>
        /// Get a guild by name
        /// </summary>
        /// <param name="guildName">Name of the guild</param>
        /// <returns></returns>
        SocketGuild GetGuild(string guildName);
        SocketGuild GetGuildById(ulong guildId);
        /// <summary>
        /// Get all the current guilds the bot is connected to
        /// </summary>
        /// <returns>List of IGuilds</returns>
        Bunch<SocketGuild> GetAllGuilds();
        /// <summary>
        /// Gets the default channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel FindDefaultChannel(SocketGuild guild);
        /// <summary>
        /// Gets a text channel with the supplied ID
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(ulong id);
        /// <summary>
        /// Gets a text channel with the supplied guild and id
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, ulong id);
        /// <summary>
        /// Gets a text channel with a supplied guild and name
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, string channelName);
        /// <summary>
        /// Gets a channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <param name="channel">Channel</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, SocketTextChannel channel);
        /// <summary>
        /// Gets all the channels of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>IReadOnlyCollection of SocketTextChannels</returns>
        Bunch<SocketTextChannel> FindGuildTextChannels(SocketGuild guild);
        /// <summary>
        /// Create an FFMPEG process for the audio commands
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Process CreateFfmpegProcess(string path);
        /// <summary>
        /// Leaves the current audio channel
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        Task LeaveAudioAsync(IGuild guild);
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="guild">Guild to send it to</param>
        /// <param name="channel">Channel to send it to</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText);
        string FindEmojis(SocketUserMessage message);
        /// <summary>
        /// Send a message through a DM
        /// </summary>
        /// <param name="user">User to DM</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendDMAsync(SocketGuildUser user, string messageText);
        string TagUser(ulong id);
        Task<IPStatus> PingHostAsync(string nameOrAddress);
        Task<IMessage> GetRandomQuoteAsync(ulong id);
        Task BackupServerAsync(ulong id, bool server);
        Task<GuildEmote> GetEmoteFromGuildAsync(ulong id, SocketGuild guild);

        Task CommandsAsync(bool includeNotImplemented);
        Task GetBotInfoAsync();
        Task StatusAsync();
        Task ChangeNicknameAsync(string guildName, string nickname);
        Task ChangeIconAsync(string imageUri);
        Task ChangeIconAsync(Uri imageUri = null);
    }
}
