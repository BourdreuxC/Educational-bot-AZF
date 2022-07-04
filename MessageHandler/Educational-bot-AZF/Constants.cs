// <copyright file="Constants.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Educational_bot_AZF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Constant class.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// App registration Id.
        /// </summary>
        public const string AppId = "bc4ba3e1-6c39-4b50-ba44-6b6b37b7fd4d";

        /// <summary>
        /// Tenant Id.
        /// </summary>
        public const string TenantId = "fefe9af7-f330-429d-8087-f5e656f7a7ce";

        /// <summary>
        /// App registration secret.
        /// </summary>
        public const string ClientSecret = "bsm8Q~Lsbk2MTKWOkFFYzM7TdAvGb-NS0Sa6XchH";

        /// <summary>
        /// Graph token scheme.
        /// </summary>
        public const string TokenScheme = "Bearer";

        /// <summary>
        /// Key of configuration corresponding to the authorized scopes of the application using Graph.
        /// </summary>
        public static readonly string[] Scopes = { "User.Read" };

        /// <summary>
        /// Conn Endpoint for the cosmosDB.
        /// </summary>
        public static readonly string CosmosConnStr = "AccountEndpoint=https://dibotaccountdev.documents.azure.com:443/;AccountKey=MkWhCZmRQle8tQbG2ur4XgJHrf4Sty7qvGGjDMJB0hxQzz6mEndN4RXdArZ3CCmrqAZS5Dc0NqSn7eo0NQwuNA==;";

        /// <summary>
        /// Database name constant.
        /// </summary>
        public const string Database = "DiiageBotDatabase";

        /// <summary>
        /// InstrumentationKey.
        /// </summary>
        public const string InstrumentationKey = "3bbb5622-4826-4128-b484-3ae7b337341a";

    }
}
