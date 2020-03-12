﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Common.DTOs.API.Giphy;
using Prawnbot.Common.DTOs.API.Overwatch;
using Prawnbot.Common.DTOs.API.Reddit;
using Prawnbot.Common.DTOs.API.Rule34;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.FileHandling.Interfaces;
using Prawnbot.Infrastructure;
using Prawnbot.Logging;
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
        private readonly IConfigUtility configUtility;

        public APIBL(IFileBL fileBL, ILogging logging, IConfigUtility configUtility)
        {
            this.fileBL = fileBL;
            this.logging = logging;
            this.configUtility = configUtility;
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
                logging.Log_Exception(e);
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
                logging.Log_Exception(e, optionalMessage: $"Error in POST request for type {typeof(T).FullName}");
                return default(T);
            }
        }

        public async Task<IBunch<GiphyDatum>> GetGifsAsync(string searchTerm, int limit = 25)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "api_key", configUtility.GiphyAPIKey },
                { "q", HttpUtility.UrlEncode(searchTerm) },
                { "limit", limit.ToString() },
                { "lang", "en" }
            };

            GiphyRootobject response = await GetRequestAsync<GiphyRootobject>(configUtility.GiphyEndpoint, parameters);
            return response.data.OrderBy(x => x.trending_datetime).ToBunch();
        }

        public async Task<IBunch<TranslateData>> TranslateAsync(string toLanguage, string fromLanguage, string textToTranslate)
        {
            if (fileBL.CheckIfTranslationExists())
            {
                return fileBL.GetTranslationFromFile(toLanguage, fromLanguage, textToTranslate);
            }
            else
            {
                object[] postData = new object[] { new { Text = textToTranslate.RemoveSpecialCharacters() } };

                string url = configUtility.MicrosoftTranslateEndpoint + "translate?api-version=3.0&to=" + toLanguage;

                if (fromLanguage != null)
                {
                    url += $"&from={fromLanguage}";
                }

                IDictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { "Ocp-Apim-Subscription-Key", configUtility.TranslateAPIKey }
                };

                return await PostRequestAsync<IBunch<TranslateData>>(url, postData, headers);
            }
        }

        public async Task<IBunch<LanguageTranslationRoot>> GetLanguagesAsync()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "api-version", "3.0" }
            };

            return await GetRequestAsync<IBunch<LanguageTranslationRoot>>(configUtility.MicrosoftTranslateEndpoint + "languages", parameters);
        }

        public async Task<bool> GetProfanityFilterAsync(string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "text", message }
            };

            return await GetRequestAsync<bool>(configUtility.ProfanityFilterEndpoint, parameters);
        }

        public async Task<IBunch<Rule34Model>> Rule34PostsAsync(string[] tags)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "tags", string.Join('+', tags) }
            };

            return await GetRequestAsync<IBunch<Rule34Model>>(configUtility.R34Endpoint + "posts", parameters);
        }

        public async Task<IBunch<Rule34Types>> Rule34TagsAsync()
        {
            return await GetRequestAsync<IBunch<Rule34Types>>(configUtility.R34Endpoint + "tags");
        }

        public async Task<IBunch<Event>> GetCalendarEntries(string calendarId)
        {
            try
            {
                // Authentication
                string clientId = configUtility.GoogleAPIKey; //From Google Developer console https://console.developers.google.com
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
                    ApplicationName = configUtility.GoogleApplicationName,
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
                logging.Log_Exception(e);
                return new Bunch<Event>();
            }
        }

        public async Task<OverwatchStats> OverwatchStatsAsync(string battletag, string region, string platform)
        {
            return await GetRequestAsync<OverwatchStats>(configUtility.OverwatchStatsEndpoint + "stats/" + platform + "/" + region + "/" + battletag + "/");
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
