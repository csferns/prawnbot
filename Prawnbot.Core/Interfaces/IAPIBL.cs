using Google.Apis.Calendar.v3.Data;
using Prawnbot.Common.DTOs.API.Giphy;
using Prawnbot.Common.DTOs.API.Overwatch;
using Prawnbot.Common.DTOs.API.Reddit;
using Prawnbot.Common.DTOs.API.Rule34;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IAPIBL
    {
        Task<IBunch<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<IBunch<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<IBunch<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<bool> GetProfanityFilterAsync(string message);
        Task<IBunch<Rule34Model>> Rule34PostsAsync(string[] tags);
        Task<IBunch<Rule34Types>> Rule34TagsAsync(); 
        Task<IBunch<Event>> GetCalendarEntries(string calendarId);
        Task<OverwatchStats> OverwatchStatsAsync(string battletag, string region, string platform);
        Task<RedditRoot> GetTopPostsBySubreddit(string subredditName, int count);
    }
}
