using Prawnbot.Core.Base;
using Prawnbot.Core.Framework;
using Prawnbot.Data.Models.API;
using System.Threading.Tasks;

namespace Prawnbot.Core.API
{
    public interface IAPIService
    {
        Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync();
    }

    public class APIService : BaseService, IAPIService
    {
        protected IAPIBl _businessLayer;

        public APIService()
        {
            _businessLayer = new APIBl();
        }

        public async Task<ListResponse<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            return LoadListResponse(await _businessLayer.GetGifsAsync(searchTerm, limit));
        }

        public async Task<ListResponse<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return LoadListResponse(await _businessLayer.TranslateAsync(toLanguage, fromLanguage, textToTranslate));
        }

        public async Task<ListResponse<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            return LoadListResponse(await _businessLayer.GetLanguagesAsync());
        }
    }
}
