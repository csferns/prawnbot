using Newtonsoft.Json;
using Prawnbot.Core.Base;
using Prawnbot.Core.Bot;
using Prawnbot.Core.LocalFileAccess;
using Prawnbot.Core.Log;
using Prawnbot.Core.Utility;
using Prawnbot.Data.Models.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Prawnbot.Core.API
{
    public interface IAPIBl
    {
        Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<List<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<bool> GetProfanityFilterAsync(string message);
    }

    public class APIBl : BaseBl, IAPIBl
    {
        private static HttpClient httpClient;

        public async Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            try
            {
                string url = "https://api.giphy.com/v1/gifs/search?api_key=" + ConfigUtility.GiphyAPIKey + "&q=" + HttpUtility.UrlEncode(searchTerm) + "&limit=" + limit + "&lang=en";

                httpClient = httpClient ?? new HttpClient();

                var httpResponse = await httpClient.GetAsync(url);
                var output = JsonConvert.DeserializeObject<GiphyRootobject>(await httpResponse.Content.ReadAsStringAsync());
                return output.data.OrderBy(x => x.trending_datetime).ToList();
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "GetGifs()", "Error getting gif", e));
                return null;
            }
        }

        public async Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            try
            {
                if (_fileBl.CheckIfTranslationExists())
                {
                    List<TranslateData> savedData = new List<TranslateData>
                    {
                        _fileBl.GetTranslationFromFile(toLanguage, fromLanguage, textToTranslate)
                    };

                    return savedData;
                }
                else
                {
                    httpClient = httpClient ?? new HttpClient();

                    object[] body = new object[] { new { Text = textToTranslate.RemoveSpecialCharacters() } };
                    var requestBody = JsonConvert.SerializeObject(body);
                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    string url = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=" + toLanguage;

                    if (fromLanguage != null) url += $"&from={fromLanguage}";

                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigUtility.TranslateAPIKey);
                    var httpResponse = await httpClient.PostAsync(url, content);

                    var converted = JsonConvert.DeserializeObject<List<TranslateData>>(await httpResponse.Content.ReadAsStringAsync());
                    await logging.PopulateTranslationLog(converted);

                    return converted;
                } 
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "TranslateAsync()", "Error in translation", e));
                return null;
            }
        }

        public async Task<List<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            try
            {
                IRestClient client = new RestClient("https://api.cognitive.microsofttranslator.com/languages?api-version=3.0");
                IRestRequest request = new RestRequest()
                {
                    Method = Method.GET,
                    RequestFormat = DataFormat.Json
                };

                IRestResponse response = await client.ExecuteTaskAsync(request);
                var converted = JsonConvert.DeserializeObject<List<LanguageTranslationRoot>>(response.Content);

                return converted;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "GetLanguagesAsync()", "Error in getting all languages", e));
                return null;
            }
        }

        public async Task<bool> GetProfanityFilterAsync(string message)
        {
            try
            {
                httpClient = httpClient ?? new HttpClient();

                string url = "https://www.purgomalum.com/service/containsprofanity?text=" + message;

                var httpResponse = await httpClient.GetAsync(url);

                bool result = bool.TryParse(await httpResponse.Content.ReadAsStringAsync(), out bool responseBool);

                return responseBool;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "Profanity", "Error in fetching profanity filter", e));
                return false;
            }
        }
    }
}
