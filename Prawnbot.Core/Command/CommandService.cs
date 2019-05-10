using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Core.Base;
using Prawnbot.Core.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prawnbot.Core.Command
{
    public interface ICommandService
    {
        Task<Response<bool>> ContainsUser(SocketCommandContext context, SocketUserMessage message);
        Task<Response<bool>> ContainsText(SocketCommandContext context, SocketUserMessage message);
        Response<string> TagUser(ulong id);
        Response<string> FlipACoin(string headsValue, string tailsValue);
        Task<Response<string[]>> YottaPrepend(SocketGuild guild);
    }

    public class CommandService : BaseService, ICommandService
    {
        protected ICommandBl _businessLayer;

        public CommandService()
        {
            _businessLayer = new CommandBl();
        }

        public async Task<Response<bool>> ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            return LoadResponse(await _businessLayer.ContainsUser(context, message));
        }

        public async Task<Response<bool>> ContainsText(SocketCommandContext context, SocketUserMessage message)
        {
            return LoadResponse(await _businessLayer.ContainsText(context, message));
        }

        public Response<string> TagUser(ulong id)
        {
            return LoadResponse(_businessLayer.TagUser(id));
        }

        public Response<string> FlipACoin(string headsValue, string tailsValue)
        {
            return LoadResponse(_businessLayer.FlipACoin(headsValue, tailsValue));
        }

        public async Task<Response<string[]>> YottaPrepend(SocketGuild guild)
        {
            return LoadResponse(await _businessLayer.YottaPrepend(guild));
        }
    }
}
