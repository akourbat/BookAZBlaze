using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedModels;

namespace FunctionsApp
{
    public static class GetBooks
    {
        [FunctionName("GetBooks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequest req,
            ILogger log)
        {
            var books = new[]
            {
                new Book { Title = "The Fog", Price = 6 },
                new Book { Title = "Apache", Price = 3 },
                new Book { Title = "Avalon", Price = 9 },
                new Book { Title = "Reactive", Price = 8 },
                new Book { Title = "MAUI", Price = 4 }

            };
            
            return new OkObjectResult(books);
        }
    }
}
