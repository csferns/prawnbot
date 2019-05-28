using Discord;
using Discord.Commands;
using Prawnbot.Core.Log;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.Module
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("translate")]
        [Summary("Translates the given text")]
        public async Task TranslateAsync(string toLanguage, [Remainder]string textToTranslate)
        {
            ILogging logging = new Logging();

            var translatedText = await _apiService.TranslateAsync(toLanguage, null, textToTranslate);
            var translation = translatedText.Entities.FirstOrDefault().translations.FirstOrDefault();

            await Context.Channel.SendMessageAsync($"{translation.text}");
            await logging.PopulateEventLog(new LogMessage(LogSeverity.Info, "Translation", translation.text));
        }
    }
}
