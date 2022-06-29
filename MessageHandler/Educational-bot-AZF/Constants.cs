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
        public const string AppId = "0093bf30-36bf-44e1-bb8c-77cd5867d67b";

        /// <summary>
        /// Tenant Id.
        /// </summary>
        public const string TenantId = "fefe9af7-f330-429d-8087-f5e656f7a7ce";

        /// <summary>
        /// App registration secret.
        /// </summary>
        public const string ClientSecret = "Lbr8Q~MGDrqclhP~bSPTEbtfcorbRFqEcjQKdcn8";

        /// <summary>
        /// Graph token scheme.
        /// </summary>
        public const string TokenScheme = "Bearer";

        /// <summary>
        /// Key of configuration corresponding to the authorized scopes of the application using Graph.
        /// </summary>
        public static readonly string[] Scopes = { "User.Read" };
    }
}
