// -----------------------------------------------------------------------
// <copyright file="CosmosQuestion.cs" company="DIIAGE">
// Copyright (c) DIIAGE 2022. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Educational_bot_AZF.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Question in the CosmosDB format.
    /// </summary>
    public class CosmosQuestion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosQuestion"/> class.
        /// </summary>
        /// <param name="id">identifier of a message question.</param>
        /// <param name="content">content of a question.</param>
        /// <param name="uid">user id of a specific question.</param>
        public CosmosQuestion(string id, string content, string uid)
        {
            this.Id = id;
            this.Content = content;
            this.AssociatedTags = new List<string>();
            this.Answers = new List<string>();
            this.UserId = uid;
        }

        /// <summary>
        /// Gets or sets the identifier of a message question | returned by the graph api.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the content of a question.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the tags for a message.
        /// </summary>
        [JsonProperty("tags")]
        public IEnumerable<string> AssociatedTags { get; set; }

        /// <summary>
        /// Gets or sets the answers of a message.
        /// </summary>
        [JsonProperty("answers")]
        public IEnumerable<string> Answers { get; set; }

        /// <summary>
        /// Gets or sets the user id of a specific question.
        /// </summary>
        [JsonProperty("uid")]
        public string UserId { get; set; }
    }
}
