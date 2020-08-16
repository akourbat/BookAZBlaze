using FunctionsApp.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

[assembly: FunctionsStartup(typeof(FunctionsApp.Startup))]

namespace FunctionsApp
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string keyVaultEndpoint = Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint");

            if (!String.IsNullOrEmpty(keyVaultEndpoint))
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = 
                    new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager())
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddEnvironmentVariables()
                    .Build();

                builder.Services.AddSingleton<IConfiguration>(config);
            }
            else
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();

                builder.Services.AddSingleton<IConfiguration>(config);
            }

            builder.Services.AddOptions<CosmosValues>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosValues").Bind(settings);
                });

            builder.Services.AddSingleton(provider =>
            {
                string connectionString = Environment.GetEnvironmentVariable("LocalCosmosDb");

                return new CosmosClientBuilder(connectionString).Build();
            });

            builder.Services.AddTransient(typeof(JsonPatchDocument<Book>));
        }
    }
}
