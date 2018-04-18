
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace EloquaHubspot2
{
    public static class Eloqua2Hubspot
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("EloquaToHubspot")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
                    
            if (!req.HasFormContentType)
                return new BadRequestObjectResult("Must pass a form type");

            string email = req.Form["Email"];
            log.Info("Form contains Email=" + email);

            if (email == null)
                return new BadRequestObjectResult("Please pass an Email form field in the request body");

            bool success = await SendToHubspot(email);
            if (!success)
                return new BadRequestObjectResult("Bad response from Hubspot");

            return new OkObjectResult("");
        }

        public static async Task<bool> SendToHubspot(string email)
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>
            {
                { "unsubscribeFromAll", true }
            };
            HttpResponseMessage response = await httpClient.PutAsJsonAsync(string.Format("https://api.hubapi.com/email/public/v1/subscriptions/" + email + "?hapikey=demo"), dictionary);
            return response.IsSuccessStatusCode;
        }
    }
}
