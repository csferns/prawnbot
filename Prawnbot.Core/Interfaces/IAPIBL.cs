using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.API.Overwatch;
using Prawnbot.Core.Model.API.Reddit;
using Prawnbot.Core.Model.API.Rule34;
using Prawnbot.Core.Model.API.Translation;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IAPIBL
    {
        Task<Bunch<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<Bunch<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<Bunch<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<bool> GetProfanityFilterAsync(string message);
        Task<Bunch<Rule34Model>> Rule34PostsAsync(string[] tags);
        Task<Bunch<Rule34Types>> Rule34TagsAsync(); 
        Task<Bunch<Event>> GetCalendarEntries(string calendarId);
        Task<OverwatchStats> OverwatchStatsAsync(string battletag, string region, string platform);
        Task<RedditRoot> GetTopPostsBySubreddit(string subredditName, int count);
    }
}
