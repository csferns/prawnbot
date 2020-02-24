using Autofac;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IBotService
    {
        Task<ResponseBase> ConnectAsync(string token, IContainer autofacContainer);
        Task<ResponseBase> DisconnectAsync(bool shutdown = false, bool switchBot = false);
        Task<ResponseBase> ReconnectAsync();
        Response<object> GetStatusAsync();
        Task<ResponseBase> SetBotRegionAsync(string regionName);
    }
}
