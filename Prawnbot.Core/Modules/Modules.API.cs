using Discord;
using Discord.Commands;
using Prawnbot.Core.Model.API.Overwatch;
using Prawnbot.Core.Model.API.Reddit;
using Prawnbot.Core.Model.API.Rule34;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Infrastructure;
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
            ListResponse<TranslateData> translatedText = await apiService.TranslateAsync(toLanguage, null, textToTranslate);
            Translation translation = translatedText.Entities.FirstOrDefault().translations.FirstOrDefault();

            await Context.Channel.SendMessageAsync($"{translation.text}");
        }

        [Command("r34-tags")]
        [Summary("Gets rule 34 tags")]
        public async Task Rule34TagsAsync()
        {
            ListResponse<Rule34Types> tags = await apiService.Rule34TagsAsync();

            StringBuilder sb = new StringBuilder();
            foreach (Rule34Types tag in tags.Entities.OrderBy(x => int.Parse(x.posts)))
            {
                sb.AppendLine($"{Format.Bold(tag.posts)} {tag.name.Replace('_', ' ')} ({string.Join(", ", tag.types)})");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("ow-stats")]
        public async Task OverwatchStatsAsync(string battletag, string region, string platform = "pc")
        {
            if (battletag.Contains('#'))
            {
                battletag.Replace('#', '-');
            }

            Response<OverwatchStats> response = await apiService.OverwatchStatsAsync(battletag, region, platform);
            OverwatchStats overwatchStat = response.Entity;

            double compWinPercentage = (overwatchStat.competitiveStats.games.played - overwatchStat.competitiveStats.games.won) * 100;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(overwatchStat.name);
            builder.WithThumbnailUrl(overwatchStat.icon);
            builder.WithDescription($"{overwatchStat.competitiveStats.games.won} comp games won out of {overwatchStat.competitiveStats.games.played} ({compWinPercentage}%)");
            builder.WithColor(Color.Green);

            await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());

        }

        [Command("top-post")]
        [Summary("Gets the top post of a specified subreddit")]
        public async Task TopPostBySubredditAsync([Remainder] string subredditName)
        {
            Response<RedditRoot> response = await apiService.GetTopPostsBySubreddit(subredditName, 1);
            
            if (response.HasData)
            {
                LowerLevelData post = response.Entity.data.children.FirstOrDefault().data;
                EmbedBuilder builder = new EmbedBuilder();

                if (post.thumbnail != null && post.thumbnail_height != null && post.thumbnail_width != null)
                {
                    builder.WithTitle("Image post");
                    builder.WithThumbnailUrl(post.thumbnail);
                }
                else
                {
                    builder.WithTitle("Text post");
                }

                builder.WithDescription($"Title: {post.title}\n" +
                                        $"Link: https://www.reddit.com{post.permalink}");
                builder.WithColor(Color.Green);

                await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Couldn't find subreddit {subredditName}");
            }
        }
    }
}
