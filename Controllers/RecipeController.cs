using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace Index.Controllers
{
    [Route("api/[controller]")]
    public class RecipeController : Controller
    {
        public IConfiguration Configuration {get;}

        public RecipeController(IConfiguration config)
        {
            Configuration = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string body)
        {
            var yummly = new YummlyClient(Configuration["YummlyAppId"],Configuration["YummlyAppKey"]);
            var recipe = await yummly.GetRecipe(body);
            var json = JObject.Parse(recipe);

            var response = new MessagingResponse();
            var message = new Message();
            message.Body($"Here's your recipe.\nName: {json["Name"].ToString()}\nCook Time:{json["TotalTime"].ToString()}\nRecipe URL:{json["RecipeUrl"].ToString()}\n\n{json["Attribution"].ToString()}");
            message.Media(new Uri(json["ImageUrl"].ToString()));
            response.Append(message);

            return new ContentResult{ Content = response.ToString(), ContentType = "application/xml" };
        }
    }
}