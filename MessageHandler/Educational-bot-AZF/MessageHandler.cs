
namespace Educational_bot_AZF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Azure.Core;
    using Azure.Identity;
    using Educational_bot_AZF.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
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
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var tenantId = Constants.TenantId;
            var clientId = Constants.AppId;
            var clientSecret = Constants.ClientSecret;

            string[] scopes = { "https://graph.microsoft.com/.default" };

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            string requestBody = string.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string messageId = data?.messageId;
            string channelId = data?.channelId;
            string teamId = data?.teamId;

            //string messageId = "1655207535262";
            //string channelId = "19:rC3Cn3ch-duulIX1tFnkDGYOse9NNzl45jpPXmPH6wU1@thread.tacv2";
            //string teamId = "1f9e5c34-74c5-48af-ac17-1989f454729f";


            var answers = await graphClient.Teams[teamId].Channels[channelId].Messages[messageId].Replies.Request().GetAsync();


            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            CosmosClient client = new(Constants.CosmosConnStr, options);
            Microsoft.Azure.Cosmos.Database database = client.GetDatabase(Constants.Database);
            Container reactionsContainer = database.GetContainer("Reactions");
            IQueryable<CosmosReaction> reactionsQuery = reactionsContainer.GetItemLinqQueryable<CosmosReaction>();
            FeedIterator<CosmosReaction> iterator = reactionsQuery.ToFeedIterator();
            FeedResponse<CosmosReaction> feedResponse = await iterator.ReadNextAsync();
            IQueryable<CosmosReaction> reactionsQueryable = feedResponse.AsQueryable();

            var reactionsValues = reactionsQueryable.ToList();
            var bestAnswer = await GetBestAnswer(answers, reactionsValues);

            Container answerContainer = database.GetContainer("Answers");
            var result = answerContainer.CreateItemAsync(bestAnswer);
        }

        private static async Task<CosmosAnswer> GetBestAnswer(IChatMessageRepliesCollectionPage answers , List<CosmosReaction> reactions)
        {
            var answerInfo = answers.ToList();
            var listInfo = new List<int>();


            for (int i = 0; i < answers.Count; i++)
            {
                var groupedReaction = answers[i].Reactions.GroupBy(r => r.ReactionType).ToList();
                var value = groupedReaction.Sum(e => e.Count() * reactions.First(r => r.Reaction.ToLower() == e.Key.ToLower()).Value);

                listInfo.Add(value);
            }
            var best = answerInfo[listInfo.IndexOf(listInfo.Max())];
            var stopby = true;
            return new CosmosAnswer( best.Id, RemoveHtml(best.Body.Content),true);
        }

        private static string RemoveHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
