using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Newtonsoft.Json;
using Ai_Chatbot.Models;
using System.Net.Http;
using System.Collections;
using Microsoft.AspNetCore.Session;
using System.Text.Json;

namespace Ai_Chatbot.Controllers
{
    public class AIController : Controller
    {
        public List<AIResponse> list=null;
        public IActionResult Index()
        {
            var templist = HttpContext.Session.GetString("Mylist");
            if (templist != null)
            {
                list = System.Text.Json.JsonSerializer.Deserialize<List<AIResponse>>(templist); 
            }
            else
            {
                list = new List<AIResponse>();
            }
            AIResponse airesponse = new AIResponse() {Result="",Message=""};
            //list.Add(airesponse);
            var jsonstr = System.Text.Json.JsonSerializer.Serialize(list);
            HttpContext.Session.SetString("Mylist", jsonstr);
            ViewBag.Mylist = list;
            return View(airesponse);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCall(AIResponse aIResponse)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:3000/ai/");

            // Serialize the message to JSON format and wrap it in StringContent
            var jsonContent = JsonConvert.SerializeObject(new { input = aIResponse.Message });
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send POST request
            var response = await client.PostAsync("chat", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                JsonDocument doc = JsonDocument.Parse(result);
                doc.RootElement.TryGetProperty("responseText", out JsonElement outstring);
                string finalresponse = outstring.ToString();
                
                var message = aIResponse.Message;

                var templist = HttpContext.Session.GetString("Mylist");
                if (templist != null)
                {
                    list = System.Text.Json.JsonSerializer.Deserialize<List<AIResponse>>(templist);
                }
                AIResponse obj=new AIResponse() {Result = finalresponse,Message=message};
                list.Add(obj);
                ViewBag.MyList= list;
                var jsonstr = System.Text.Json.JsonSerializer.Serialize(list);
                HttpContext.Session.SetString("Mylist", jsonstr);
                return View("index",obj);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Failed to get a response from AI endpoint.");
            }

        }
    }
}
