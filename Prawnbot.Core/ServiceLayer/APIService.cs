using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using Prawnbot.Data.Models.API;
using System.Threading.Tasks;

namespace Prawnbot.Core.ServiceLayer
{
    public interface IAPIService
    {
        Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<ListResponse<Event>> GetCalendarEntries(string calendarId);
    }

    public class APIService : BaseService, IAPIService
    {
        protected IAPIBl _apiBl;

        public APIService()
        {
            _apiBl = new APIBl();
        }

        public async Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            return LoadListResponse(await _apiBl.GetGifsAsync(searchTerm, limit));
        }

        public async Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return LoadListResponse(await _apiBl.TranslateAsync(toLanguage, fromLanguage, textToTranslate));
        }

        public async Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            return LoadListResponse(await _apiBl.GetLanguagesAsync());
        }

        public async Task<ListResponse<Event>> GetCalendarEntries(string calendarId)
        {
            return LoadListResponse(await _apiBl.GetCalendarEntries(calendarId)); 
        }
    }
}
