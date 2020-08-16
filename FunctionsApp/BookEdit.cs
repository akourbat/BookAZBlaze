using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using FunctionsApp.Models;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace FunctionsApp
{
    public class BookEdit : ControllerBase
    {
        public BookEdit()
        {

        }

        [FunctionName("BookEdit")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "books/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string input = await new StreamReader(req.Body).ReadToEndAsync();
            var patch1 = JsonConvert.DeserializeObject<JsonPatchDocument<Book>>(input);

            //using var streamReader = new StreamReader(req.Body);
            //using var jsonReader = new JsonTextReader(streamReader);
            //JsonSerializer serializer = new JsonSerializer();
            //JsonPatchDocument<Book> patch = new JsonPatchDocument<Book>();

            //try
            //{
            //    patch = serializer.Deserialize<JsonPatchDocument<Book>>(jsonReader);
            //}
            //catch (JsonReaderException) { }
            
            var original = new Book { Id = "fc62d1f9-3003-4d13-b90c-797108b8c2f3", Title = "Nada", Price = 2 };
            patch1.ApplyTo(original);

            // var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

            return new OkObjectResult(original);
        }
    }
}
