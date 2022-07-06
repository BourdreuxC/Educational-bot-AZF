// <copyright file="QnAKnowledgebaseTrainer.cs" company="DiiBot team">
// Copyright (c) DiiBot team. All rights reserved.
// </copyright>

namespace Educational_bot_AZF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Educational_bot_AZF.Models;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Triggered on Cosmos update and feed the Qna with informations.
    /// </summary>
    public static class QnAKnowledgebaseTrainer
    {
        /// <summary>
        /// Triggered on insert or update.
        /// </summary>
        /// <param name="input">Values.</param>
        /// <param name="log">Logger.</param>
        /// <returns>Task to run.</returns>
        [FunctionName("QnAKnowledgebaseTrainer")]
        public static async Task Run(
            [CosmosDBTrigger(
            databaseName: "DiiageBotDatabase",
            collectionName: "Answers",
            ConnectionStringSetting = "CosmosConnStr",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            ILogger log)
        {
            try
            {
                if (input != null && input.Count > 0)
                {
                    var answer = input.First(a => a.GetPropertyValue<bool>("BestAnswer"));
                    if (answer != null)
                    {
                        CosmosQuestion question = await GetAssociatedQuestion(answer);
                        await FeedQnA(answer, question);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieve the CosmosEntity that is associated to the answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <returns>The Question.</returns>
        private static async Task<CosmosQuestion> GetAssociatedQuestion(Document answer)
        {
            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            CosmosClient client = new (Constants.CosmosConnStr, options);
            Microsoft.Azure.Cosmos.Database database = client.GetDatabase(Constants.Database);
            Container questionContainer = database.GetContainer("Questions");
            IQueryable<CosmosQuestion> questionQuery = questionContainer.GetItemLinqQueryable<CosmosQuestion>();
            FeedIterator<CosmosQuestion> iterator = questionQuery.ToFeedIterator();
            FeedResponse<CosmosQuestion> feedResponse = await iterator.ReadNextAsync();
            IQueryable<CosmosQuestion> questions = feedResponse.AsQueryable();

            CosmosQuestion associatedQuestion = questions.First(q => q.Answers.Any(a => a == answer.Id));
            return associatedQuestion;
        }

        /// <summary>
        /// Start the process of updating the QnA.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="question">The Question.</param>
        /// <returns>Task to resolve.</returns>
        private static async Task FeedQnA(Document answer, CosmosQuestion question)
        {
            string key = "b93f3a2f6496424482be4d8e700d1b31";
            var kbid = "1b6c2080-f7a3-4f05-bbc3-ac50a2ed652a";
            QnAMakerClient qnaClient = new (new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = "https://diibot-qna-maker.cognitiveservices.azure.com/",
            };

            var keys = await qnaClient.EndpointKeys.GetKeysAsync();
            var queryingURL = "https://diibot-qna-maker.azurewebsites.net";
            var qnaRuntimeCli = new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(keys.PrimaryEndpointKey)) { RuntimeEndpoint = queryingURL };
            var checkAnswerQuery = await qnaRuntimeCli.Runtime.GenerateAnswerAsync(kbid, new QueryDTO { Question = question.Content });
            var answerText = answer.GetPropertyValue<string>("content");
            if (checkAnswerQuery.Answers[0].Answer == answerText)
            {
                await UpdateQnA(answer, question, qnaClient, kbid);
            }
            else
            {
                await AddNewQnA(answer, question, qnaClient, kbid);
            }

            await qnaRuntimeCli.Runtime.TrainAsync(kbid, new FeedbackRecordsDTO());
            await qnaClient.Knowledgebase.PublishAsync(kbid);
        }

        /// <summary>
        /// When the answer does not already exist, insert it.
        /// </summary>
        /// <returns>Task to resolve.</returns>
        private static async Task AddNewQnA(Document answer, CosmosQuestion question, QnAMakerClient qnaClient, string kbid)
        {
            UpdateKbOperationDTOAdd updateKb = new ()
            {
                QnaList = new List<QnADTO>
                {
                    new QnADTO()
                    {
                        Questions = new List<string> { question.Content },
                        Answer = answer.GetPropertyValue<string>("content"),
                    },
                },
            };

            UpdateKbOperationDTO update = new ()
            {
                Add = updateKb,
            };
            await qnaClient.Knowledgebase.UpdateAsync(kbid, update);
        }

        /// <summary>
        /// When the answer does already exist add a new variant to the questions.
        /// </summary>
        /// <returns>Task to resolve.</returns>
        private static async Task UpdateQnA(Document answer, CosmosQuestion question, QnAMakerClient qnaClient, string kbid)
        {
            var down = await qnaClient.Knowledgebase.DownloadAsync(kbid, "Prod");
            var answersInfos = down.QnaDocuments.FirstOrDefault(qnaD => qnaD.Answer == answer.GetPropertyValue<string>("content"));
            answersInfos.Questions.Add(question.Content);

            var replace = new ReplaceKbDTO
            {
                QnAList = down.QnaDocuments,
            };

            await qnaClient.Knowledgebase.ReplaceAsync(kbid, replace);
        }
    }
}
