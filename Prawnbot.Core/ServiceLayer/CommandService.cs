using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
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
        protected ICommandBl _commandBl;

        public CommandService()
        {
            _commandBl = new CommandBl();
        }

        public async Task<Response<bool>> ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            return LoadResponse(await _commandBl.ContainsUser(context, message));
        }

        public async Task<Response<bool>> ContainsText(SocketCommandContext context, SocketUserMessage message)
        {
            return LoadResponse(await _commandBl.ContainsText(context, message));
        }

        public Response<string> TagUser(ulong id)
        {
            return LoadResponse(_commandBl.TagUser(id));
        }

        public Response<string> FlipACoin(string headsValue, string tailsValue)
        {
            return LoadResponse(_commandBl.FlipACoin(headsValue, tailsValue));
        }

        public async Task<Response<string[]>> YottaPrepend(SocketGuild guild)
        {
            return LoadResponse(await _commandBl.YottaPrepend(guild));
        }
    }
}
