
namespace Educational_bot_AZF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Core;
    using Azure.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Microsoft.Identity.Client;
    using Newtonsoft.Json;

    public static class MessageHandler
    {

        [FunctionName("MessageHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var tenantId = Constants.TenantId;
            var clientId = Constants.AppId;
            var clientSecret = Constants.ClientSecret;

            string[] scopes = { "https://graph.microsoft.com/.default" };

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            string messageId = "1655207535262";
            string channelId = "19:rC3Cn3ch-duulIX1tFnkDGYOse9NNzl45jpPXmPH6wU1@thread.tacv2";
            string teamId = "1f9e5c34-74c5-48af-ac17-1989f454729f";


            var answers = await graphClient.Teams[teamId].Channels[channelId].Messages[messageId].Replies.Request().GetAsync();


            return new OkObjectResult(answers);
        }
    }
}
