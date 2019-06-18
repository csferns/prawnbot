using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Prawnbot.Core.Utility;
using Prawnbot.Data.Models.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IAPIBl
    {
        Task<T> GetRequestAsync<T>(string url);
        Task<T> PostRequestAsync<T>(string url, object postData, Dictionary<string, string> headers);
        Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25);
        Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate);
        Task<List<LanguageTranslationRoot>> GetLanguagesAsync();
        Task<bool> GetProfanityFilterAsync(string message);
        Task<List<Rule34Model>> Rule34Async(string[] tags);
        Task<List<Event>> GetCalendarEntries(string calendarId);
    }

    public class APIBl : BaseBl, IAPIBl
    {
        private static HttpClient httpClient;
         
        public async Task<T> GetRequestAsync<T>(string url)
        {
            try
            {
                httpClient = httpClient ?? new HttpClient();

                HttpResponseMessage httpResponse = await httpClient.GetAsync(url);

                T responseObject = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
                return responseObject;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "", $"Error in GET request for type {typeof(T).FullName}", e));
                return default(T);
            }
        }

        public async Task<T> PostRequestAsync<T>(string url, object postData, Dictionary<string, string> headers)
        {
            try
            {
                httpClient = httpClient ?? new HttpClient();

                string requestBody = JsonConvert.SerializeObject(postData);
                StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                foreach (KeyValuePair<string, string> header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

                T responseObject = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
                return responseObject;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new Discord.LogMessage(Discord.LogSeverity.Error, "", $"Error in POST request for type {typeof(T).FullName}", e));
                return default(T);
            }
        }

        public async Task<List<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            GiphyRootobject response = await GetRequestAsync<GiphyRootobject>("https://api.giphy.com/v1/gifs/search?api_key=" + ConfigUtility.GiphyAPIKey + "&q=" + HttpUtility.UrlEncode(searchTerm) + "&limit=" + limit + "&lang=en");
            return response.data.OrderBy(x => x.trending_datetime).ToList();
        }

        public async Task<List<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            if (_fileBl.CheckIfTranslationExists())
            {
                return _fileBl.GetTranslationFromFile(toLanguage, fromLanguage, textToTranslate);
            }
            else
            {
                object[] postData = new object[] { new { Text = textToTranslate.RemoveSpecialCharacters() } };

                string url = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=" + toLanguage;
                if (fromLanguage != null) url += $"&from={fromLanguage}";

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { "Ocp-Apim-Subscription-Key", ConfigUtility.TranslateAPIKey }
                };

                return await PostRequestAsync<List<TranslateData>>(url, postData, headers);
            } 
        }

        public async Task<List<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            return await GetRequestAsync<List<LanguageTranslationRoot>>("https://api.cognitive.microsofttranslator.com/languages?api-version=3.0");
        }

        public async Task<bool> GetProfanityFilterAsync(string message)
        {
            return await GetRequestAsync<bool>("https://www.purgomalum.com/service/containsprofanity?text=" + message);
        }

        public async Task<List<Rule34Model>> Rule34Async(string[] tags)
        {
            return await GetRequestAsync<List<Rule34Model>>("https://r34-json-api.herokuapp.com/posts?tags=" + string.Join('+', tags));
        }

        public async Task<List<Event>> GetCalendarEntries(string calendarId)
        {
            string[] scopes = { CalendarService.Scope.CalendarEventsReadonly };

            UserCredential credential;

            using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            CalendarService service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ConfigUtility.GoogleApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = await request.ExecuteAsync();

            if (events.Items != null && events.Items.Count > 0)
            {
                return events.Items.ToList();
            }
            else
            {
                return new List<Event>();
            }
        }
    }
}
