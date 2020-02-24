using System;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine.Interfaces
{
    public interface ICommandEngine
    {
        Task BeginListen(Func<string> listenAction);
    }
}
