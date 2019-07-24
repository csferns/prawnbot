using Discord;
using Discord.Commands;
using Prawnbot.Core.Log;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Modules
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("translate")]
        [Summary("Translates the given text")]
        public async Task TranslateAsync(string toLanguage, [Remainder]string textToTranslate)
        {
            var translatedText = await apiService.TranslateAsync(toLanguage, null, textToTranslate);
            var translation = translatedText.Entities.FirstOrDefault().translations.FirstOrDefault();

            await Context.Channel.SendMessageAsync($"{translation.text}");
            //await logging.PopulateEventLog(new LogMessage(LogSeverity.Info, "Translation", translation.text));
        }

        [Command("r34tags")]
        [Summary("Gets rule 34 tags")]
        public async Task Rule34TagsAsync()
        {
            var tags = await apiService.Rule34TagsAsync();

            StringBuilder sb = new StringBuilder();
            foreach (var tag in tags.Entities.OrderBy(x => int.Parse(x.posts)))
            {
                sb.AppendLine($"{Format.Bold(tag.posts)} {tag.name.Replace('_', ' ')} ({string.Join(", ", tag.types)})");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }
    }
}
