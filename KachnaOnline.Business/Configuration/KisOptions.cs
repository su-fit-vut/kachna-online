// KisOptions.cs
// Author: Ondřej Ondryáš

using System.Collections.Generic;

namespace KachnaOnline.Business.Configuration
{
    public class KisOptions
    {
        /// <summary>
        /// The base URL of KIS API.
        /// </summary>
        /// <example>
        /// Typically the value would be either https://su-int.fit.vutbr.cz/kis/api/
        /// or https://su-dev.fit.vutbr.cz/kis/api/.
        /// </example>
        public string ApiBaseUrl { get; set; }
        
        /// <summary>
        /// A KIS display token.
        /// </summary>
        /// <remarks>
        /// A display token is a long-lived token issued by KIS to a user with scope 'oo'
        /// that can be used to fetch certain range of information including product list,
        /// current beers on tap or prestige leaderboard. 
        /// </remarks>
        public string DisplayToken { get; set; }
        
        /// <summary>
        /// A dictionary that controls how KIS roles are mapped to local roles.
        /// </summary>
        /// <example>
        /// Configuring this as
        /// <code>
        /// "RoleMappings": {
        ///   "regular_member": [
        ///     "StatesManager",
        ///     "EventsManager",
        ///     "BoardGamesManager"
        ///   ]
        /// }
        /// </code>
        /// grants local roles StatesManager, EventsManager and BoardGamesManager
        /// to all users with KIS role regular_member.
        /// </example>
        public Dictionary<string, List<string>> RoleMappings { get; set; }
    }
}
