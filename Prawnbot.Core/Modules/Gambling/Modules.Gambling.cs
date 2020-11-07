using Discord.Commands;
using Prawnbot.Core.Attributes;
using Prawnbot.Core.Modules.Gambling;
using System.Threading.Tasks;

namespace Prawnbot.Core.Modules
{
    public partial class Modules
    {
        [Command("coinflip")]
        [Summary("Flips a coin")]
        public async Task FlipACoinAsync(string headsValue = null, string tailsValue = null)
        {
            CoinFlip coinFlip = new CoinFlip();
            bool heads = coinFlip.Execute();

            await Context.Channel.SendMessageAsync("Flipping coin...");
            await Task.Delay(1000);
            await Context.Channel.SendMessageAsync(heads ? headsValue ?? "Heads" : tailsValue ?? "Tails");
        }
    }
}
