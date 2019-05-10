using Newtonsoft.Json;
using Prawnbot.Core.Base;
using Prawnbot.Core.Log;
using Prawnbot.Data.Models.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Prawnbot.Core.API
{
    public interface IAPIBl
    {
        Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<List<LanguageTranslationRoot>> GetLanguagesAsync();
    }

    public class APIBl : BaseBl, IAPIBl
    {
        public async Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            try
            {
                RestClient client = new RestClient("https://api.giphy.com/v1/gifs/search?api_key=3oD1V4aTpQNqu8yCilNQSjx8WWrsk9Bs&q=" + HttpUtility.UrlEncode(searchTerm) + "&limit=" + limit + "&offset=0&rating=G&lang=en");
                RestRequest request = new RestRequest
                {
                    Method = Method.GET,
                    RequestFormat = DataFormat.Json
                };

                IRestResponse response = await client.ExecuteTaskAsync(request);
                var output = JsonConvert.DeserializeObject<GiphyRootobject>(response.Content);
                return output.data.OrderBy(x => x._score).ThenBy(x => x.trending_datetime).ToList();
            }
            catch (Exception e)
            {
                await Logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "GetGifs()", "Error getting gif", e));
                return null;
            }
        }

        public async Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            try
            {
                string host = "https://api.cognitive.microsofttranslator.com";
                string route = "/translate?api-version=3.0&to=" + toLanguage;

                if (fromLanguage != null) route += $"&from={fromLanguage}";

                object[] body = new object[] { new { Text = HttpUtility.UrlEncode(textToTranslate) } };
                var requestBody = JsonConvert.SerializeObject(body);

                RestClient client = new RestClient(host + route);
                RestRequest request = new RestRequest
                {
                    Method = Method.POST,
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Ocp-Apim-Subscription-Key", "eb4e705d0e2949988f2bc6543e2f505e");
                request.AddJsonBody(requestBody);

                IRestResponse response = await client.ExecuteTaskAsync(request);
                var converted = JsonConvert.DeserializeObject<List<TranslateData>>(response.Content);

                return converted;
            }
            catch (Exception e)
            {
                await Logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "TranslateAsync()", "Error in translation", e));
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
                await Logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "TranslateAsync()", "Error in translation", e));
                return null;
            }
        }
    }
}
