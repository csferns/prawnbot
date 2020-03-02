using Google.Apis.Calendar.v3.Data;
using Prawnbot.Common.DTOs.API.Giphy;
using Prawnbot.Common.DTOs.API.Overwatch;
using Prawnbot.Common.DTOs.API.Reddit;
using Prawnbot.Common.DTOs.API.Rule34;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface IAPIService
    {
        Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<ListResponse<Event>> GetCalendarEntries(string calendarId);
        Task<ListResponse<Rule34Types>> Rule34TagsAsync();
        Task<Response<OverwatchStats>> OverwatchStatsAsync(string battletag, string region, string platform);
        Task<Response<RedditRoot>> GetTopPostsBySubreddit(string subredditName, int count);
    }
}
