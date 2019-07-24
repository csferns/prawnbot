using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.API.Rule34;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IAPIService
    {
        Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<ListResponse<Event>> GetCalendarEntries(string calendarId);
        Task<ListResponse<Rule34Types>> Rule34TagsAsync();
    }

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
    }
}