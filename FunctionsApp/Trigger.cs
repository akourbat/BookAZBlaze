using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using FunctionsApp.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace FunctionsApp
{
    public class Trigger
    {
        private readonly IServiceProvider _provider;

        private readonly CosmosClient _client;

        private readonly CosmosValues _settings;

        public Trigger(IServiceProvider provider, CosmosClient client, IOptions<CosmosValues> options)
        {
            _provider = provider;
            _client = client;
            _settings = options.Value;
        }

        [FunctionName("ChangeFeedProcessor")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "TabletDb",
            collectionName: "PatchEvents",
            ConnectionStringSetting = "LocalCosmosDb",
            FeedPollDelay = 1000,
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input, ILogger log)
        {
            var container = _client.GetContainer(_settings.Database, _settings.OutputCollection);

            if (input != null && input.Count > 0)
            {
                foreach (var doc in input)
                {
                    //string patchTypeName = doc.GetPropertyValue<string>("TargetType");
                    //Type typeArg = Assembly.GetExecutingAssembly().GetType(patchTypeName);
                    //Type generic = typeof(JsonPatchDocument<>);
                    //Type genericTyped = generic.MakeGenericType(typeArg);
                    //var typedJsonPatchInstance = _provider.GetService(genericTyped);
                    //var aa = typedJsonPatchInstance.GetType();

                    //Type constructed = generic.MakeGenericType(typeArg);
                    //var instance = Activator.CreateInstance(constructed);

                    string targetId = doc.GetPropertyValue<string>("TargetId");
                    string targetPk = doc.GetPropertyValue<string>("TargetPK");


                    JsonPatchDocument<Book> patch  = doc.GetPropertyValue<JsonPatchDocument<Book>>("Patch");

                    var bookFromDb = await container.ReadItemAsync<Book>(targetId, new PartitionKey(targetPk));

                    var book = bookFromDb.Resource;

                    patch.ApplyTo(book);

                    var response = await container.ReplaceItemAsync<Book>(book, targetId, new PartitionKey(targetPk));

                    log.LogWarning("Documents modified " + response.Resource.Id);
                }
                log.LogInformation("Documents modified " + input.Count);
            }
        }
    }
}
