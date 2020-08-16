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
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.SignalR;

namespace FunctionsApp
{
    public class SignalRBookHub : ServerlessHub
    {
        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
        {
            return Negotiate();
        }

        //[FunctionName(nameof(OnConnected))]
        //public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
        //{
        //    await Clients.All.SendAsync("ClientConnected", invocationContext.ConnectionId);
        //    logger.LogInformation($"{invocationContext.ConnectionId} has connected");
        //}

        //[FunctionName(nameof(Broadcast))]
        //public async Task Broadcast([SignalRTrigger] InvocationContext invocationContext, string message, ILogger logger)
        //{
        //    await Clients.All.SendAsync("ReceiveBroadcast", new SignalRMessage() { Arguments = new[] { message } });
        //    logger.LogInformation($"{invocationContext.ConnectionId} broadcast {message}");
        //}

        //[FunctionName(nameof(OnDisconnected))]
        //public void OnDisconnected([SignalRTrigger] InvocationContext invocationContext)
        //{
        //}
    }


    public static class CreateBook
    {
        [FunctionName("CreateBook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "books")] HttpRequest req,
            [SignalR(HubName = "SignalRBookHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Book book = JsonConvert.DeserializeObject<Book>(requestBody);

            if (book is null)
            {
                return new BadRequestResult();
            }
            await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "BookUpdate",
                    Arguments = new[] { book }
                });

            return new OkObjectResult(book);
        }
    }
}
