using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    /// <summary>
    /// Methods that relate to the handling of console commands
    /// </summary>
    public interface IConsoleBL
    {
        bool ValidCommand(string command);
        Task<bool> HandleConsoleCommand(string command);
    }
}
