using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.API.Overwatch;
using Prawnbot.Core.Model.API.Reddit;
using Prawnbot.Core.Model.API.Rule34;
using Prawnbot.Core.Model.API.Translation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IAPIBL
    {
        Task<HashSet<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<HashSet<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<HashSet<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<bool> GetProfanityFilterAsync(string message);
        Task<HashSet<Rule34Model>> Rule34PostsAsync(string[] tags);
        Task<HashSet<Rule34Types>> Rule34TagsAsync(); 
        Task<HashSet<Event>> GetCalendarEntries(string calendarId);
        Task<OverwatchStats> OverwatchStatsAsync(string battletag, string region, string platform);
        Task<RedditRoot> GetTopPostsBySubreddit(string subredditName, int count);
    }
}
