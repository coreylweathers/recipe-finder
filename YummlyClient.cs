using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Index
{
    public class YummlyClient
    {
        public string AppId {get; private set;}
        public string AppKey {get; private set;}
        internal HttpClient httpClient;
        private const string BASE_URL = "https://api.yummly.com/v1";

        public YummlyClient(string appId = null, string appKey = null)
        {
            AppId = appId ?? "REPLACE_YOUR_APP_ID_HERE";
            AppKey = appKey ?? "REPLACE_YOUR_APP_KEY_HERE";
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Yummly-App-ID", AppId);
            httpClient.DefaultRequestHeaders.Add("X-Yummly-App-Key", AppKey);
        }

        public async Task<string> GetRecipe(string query)
        {
            string result = null;
            try
            {
                var recipeId = await SearchForRecipeId(query);
                var uriBuilder = new UriBuilder($"{BASE_URL}/api/recipe/{recipeId}");
                var json = JObject.Parse(await httpClient.GetStringAsync(uriBuilder.ToString()));
                var element = JObject.FromObject( new {
                    Name = json["name"].ToString(),
                    TotalTime = json["totalTime"].ToString(),
                    ImageUrl = json["images"][0]["hostedLargeUrl"].ToString(),
                    RecipeUrl = json["source"]["sourceRecipeUrl"].ToString(),
                    Attribution = json["attribution"]["text"].ToString()
                });
                result = element.ToString();
            }
            catch(Exception)
            {
                result = null;
            }

            return result;
        }

        protected async Task<string> SearchForRecipeId(string query)
        {
            string result = String.Empty;
            var uriBuilder= new UriBuilder($"{BASE_URL}/api/recipes");
            uriBuilder.Query = $"q={query}";
            try
            {
                var json = JObject.Parse(await httpClient.GetStringAsync(uriBuilder.ToString()));
                int index = new Random().Next(json["matches"].Children().Count());
                var selected = json["matches"].Children().ElementAtOrDefault(index);
                result = selected["id"].ToString();
            }
            catch
            {
                result = String.Empty;
            }

            return result;
        }
    }
}