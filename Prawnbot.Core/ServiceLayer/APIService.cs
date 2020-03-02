using Google.Apis.Calendar.v3.Data;
using Prawnbot.Common.DTOs.API.Giphy;
using Prawnbot.Common.DTOs.API.Overwatch;
using Prawnbot.Common.DTOs.API.Reddit;
using Prawnbot.Common.DTOs.API.Rule34;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public class APIService : BaseService, IAPIService
    {
        private readonly IAPIBL apiBL;

        public APIService(IAPIBL apiBL)
        {
            this.apiBL = apiBL;
        }

        public async Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            return LoadListResponse(await apiBL.GetGifsAsync(searchTerm, limit));
        }

        public async Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return LoadListResponse(await apiBL.TranslateAsync(toLanguage, fromLanguage, textToTranslate));
        }

        public async Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            return LoadListResponse(await apiBL.GetLanguagesAsync());
        }

        public async Task<ListResponse<Event>> GetCalendarEntries(string calendarId)
        {
            return LoadListResponse(await apiBL.GetCalendarEntries(calendarId));
        }

        public async Task<ListResponse<Rule34Types>> Rule34TagsAsync()
        {
            return LoadListResponse(await apiBL.Rule34TagsAsync());
        }

        public async Task<Response<OverwatchStats>> OverwatchStatsAsync(string battletag, string region, string platform)
        {
            return LoadResponse(await apiBL.OverwatchStatsAsync(battletag, region, platform));
        }

        public async Task<Response<RedditRoot>> GetTopPostsBySubreddit(string subredditName, int count)
        {
            return LoadResponse(await apiBL.GetTopPostsBySubreddit(subredditName, count));
        }
    }
}