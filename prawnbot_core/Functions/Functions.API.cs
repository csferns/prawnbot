using Newtonsoft.Json;
using prawnbot_core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Functions
    {
		public async Task<GifyDatum[]> GetGif(string searchTerm)
        {
            try
            {
                var client = new RestClient("https://api.giphy.com/v1/gifs/search?api_key=3oD1V4aTpQNqu8yCilNQSjx8WWrsk9Bs&q=" + searchTerm + "&limit=25&offset=0&rating=G&lang=en");
                var request = new RestRequest
                {
                    Method = Method.GET,
                    RequestFormat = DataFormat.Json
                };

                var response = await client.ExecuteTaskAsync(request);
                var output = JsonConvert.DeserializeObject<GifyRootobject>(response.Content);
                return output.data.OrderBy(x => x._score).ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
