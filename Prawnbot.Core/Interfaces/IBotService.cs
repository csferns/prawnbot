using Autofac;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IBotService
    {
        Task<ResponseBase> ConnectAsync(string token, IContainer autofacContainer);
        Task<ResponseBase> DisconnectAsync(bool shutdown = false);
        Task<ResponseBase> ReconnectAsync();
        Response<object> GetStatus();
        Task<ResponseBase> SetBotRegionAsync(string regionName);
    }
}
