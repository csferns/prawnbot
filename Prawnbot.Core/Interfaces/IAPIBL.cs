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
