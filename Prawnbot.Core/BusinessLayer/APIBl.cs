using Discord;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.API.Overwatch;
using Prawnbot.Core.Model.API.Reddit;
using Prawnbot.Core.Model.API.Rule34;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Prawnbot.Core.BusinessLayer
{
    public class APIBL : BaseBL, IAPIBL
    {
        private readonly IFileBL fileBL;
        private readonly ILogging logging;

        public APIBL(IFileBL fileBL, ILogging logging)
        {
            this.fileBL = fileBL;
            this.logging = logging;
        }

        private async Task<T> GetRequestAsync<T>(string url, IDictionary<string, string> parameters = null)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder(url);
                NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (parameters != null && parameters.Any())
                {
                    foreach (KeyValuePair<string, string> parameter in parameters)
                    {
                        query[parameter.Key] = parameter.Value;
                    }
                }

                uriBuilder.Query = query.ToString();

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage httpResponse = await client.GetAsync(uriBuilder.ToString());

                    T responseObject = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
                    return responseObject;
                }
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
                return default(T);
            }
        }

        private async Task<T> PostRequestAsync<T>(string url, object postData, IDictionary<string, string> headers)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestBody = JsonConvert.SerializeObject(postData);
                    StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    HttpResponseMessage httpResponse = await client.PostAsync(url, content);

                    T responseObject = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
                    return responseObject;
                }
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e, optionalMessage: $"Error in POST request for type {typeof(T).FullName}");
                return default(T);
            }
        }

        public async Task<Bunch<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "api_key", ConfigUtility.GiphyAPIKey },
                { "q", HttpUtility.UrlEncode(searchTerm) },
                { "limit", limit.ToString() },
                { "lang", "en" }
            };

            GiphyRootobject response = await GetRequestAsync<GiphyRootobject>(ConfigUtility.GiphyEndpoint, parameters);
            return response.data.OrderBy(x => x.trending_datetime).ToBunch();
        }

        public async Task<Bunch<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            if (fileBL.CheckIfTranslationExists())
            {
                return fileBL.GetTranslationFromFile(toLanguage, fromLanguage, textToTranslate);
            }
            else
            {
                object[] postData = new object[] { new { Text = textToTranslate.RemoveSpecialCharacters() } };

                string url = ConfigUtility.MicrosoftTranslateEndpoint + "translate?api-version=3.0&to=" + toLanguage;

                if (fromLanguage != null)
                {
                    url += $"&from={fromLanguage}";
                }

                IDictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { "Ocp-Apim-Subscription-Key", ConfigUtility.TranslateAPIKey }
                };

                return await PostRequestAsync<Bunch<TranslateData>>(url, postData, headers);
            }
        }

        public async Task<Bunch<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "api-version", "3.0" }
            };

            return await GetRequestAsync<Bunch<LanguageTranslationRoot>>(ConfigUtility.MicrosoftTranslateEndpoint + "languages", parameters);
        }

        public async Task<bool> GetProfanityFilterAsync(string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "text", message }
            };

            return await GetRequestAsync<bool>(ConfigUtility.ProfanityFilterEndpoint, parameters);
        }

        public async Task<Bunch<Rule34Model>> Rule34PostsAsync(string[] tags)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "tags", string.Join('+', tags) }
            };

            return await GetRequestAsync<Bunch<Rule34Model>>(ConfigUtility.R34Endpoint + "posts", parameters);
        }

        public async Task<Bunch<Rule34Types>> Rule34TagsAsync()
        {
            return await GetRequestAsync<Bunch<Rule34Types>>(ConfigUtility.R34Endpoint + "tags");
        }

        public async Task<Bunch<Event>> GetCalendarEntries(string calendarId)
        {
            try
            {
                // Authentication
                string clientId = ConfigUtility.GoogleAPIKey; //From Google Developer console https://console.developers.google.com
                string clientSecret = ""; //From Google Developer console https://console.developers.google.com
                string userName = ""; // A string used to identify a user.
                string[] scopes = new string[] {
                    CalendarService.Scope.Calendar, // Manage your calendars
 	                CalendarService.Scope.CalendarReadonly // View your Calendars
                };

                // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }, scopes, userName, CancellationToken.None, new FileDataStore("Daimto.GoogleCalendar.Auth.Store")).Result;

                // Create the service.
                using (CalendarService service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ConfigUtility.GoogleApplicationName,
                }))
                {
                    // Define parameters of request.
                    EventsResource.ListRequest request = service.Events.List(calendarId);
                    request.TimeMin = DateTime.Now;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 10;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                    // List events.
                    Events events = await request.ExecuteAsync();
                    return events.Items.ToBunch();
                }
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
                return new Bunch<Event>();
            }
        }

        public async Task<OverwatchStats> OverwatchStatsAsync(string battletag, string region, string platform)
        {
            return await GetRequestAsync<OverwatchStats>(ConfigUtility.OverwatchStatsEndpoint + "stats/" + platform + "/" + region + "/" + battletag + "/");
        }

        public async Task<RedditRoot> GetTopPostsBySubreddit(string subredditName, int count)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "count", count.ToString() }
            };

            return await GetRequestAsync<RedditRoot>("https://www.reddit.com/r/" + subredditName + "/top/.json", parameters);
        }
    }
}
