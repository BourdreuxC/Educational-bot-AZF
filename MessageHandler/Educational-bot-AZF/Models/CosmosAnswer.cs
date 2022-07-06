// -----------------------------------------------------------------------
// <copyright file="CosmosAnswer.cs" company="DIIAGE">
// Copyright (c) DIIAGE 2022. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Educational_bot_AZF.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Answer in the cosmos format.
    /// </summary>
    public class CosmosAnswer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosAnswer"/> class.
        /// </summary>
        /// <param name="id">identifier of a message answer.</param>
        /// <param name="content"> content of an answer.</param>
        /// <param name="bestA"> is the best answer.</param>
        public CosmosAnswer(string id, string content, bool bestA)
        {
            this.Id = id;
            this.Content = content;
            this.Reactions = new List<string>();
            this.BestAnswer = bestA;
        }

        /// <summary>
        /// Gets or sets the identifier of a message answer | returned by the graph api.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the content of an answer.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the user id of a specific answer.
        /// </summary>
        [JsonProperty("uid")]
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the reactions to a message.
        /// </summary>
        [JsonProperty("reactions")]
        public IEnumerable<string> Reactions { get; set; }

        /// <summary>
        /// Gets or sets the informations about the fact that a message is the best answer.
        /// </summary>
        [JsonProperty("BestAnswer")]
        public bool? BestAnswer { get; set; }
    }
}
