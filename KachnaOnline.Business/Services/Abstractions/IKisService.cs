using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.Kis;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IKisService
    {
        /// <summary>
        /// Exchanges the given KIS <paramref name="sessionId"/> for a KIS authorization and refresh token that
        /// represent a user's identity. Uses the authorization token to fetch information about the user. Returns the
        /// tokens and the user data in a <see cref="KisIdentity"/> object.
        /// </summary>
        /// <param name="sessionId">A KIS session ID.</param>
        /// <returns>A <see cref="KisIdentity"/> object with the KIS tokens and user data.
        /// If KIS returns 404 Not Found, the object has all its properties set to null.
        /// If another error occurs, null is returned.</returns>
        Task<KisIdentity> GetIdentityFromSession(string sessionId);

        /// <summary>
        /// Uses the given KIS <paramref name="refreshToken"/> to fetch a new KIS authorization token.
        /// Uses the authorization token to fetch information about the user. Returns the tokens and the user data in
        /// a <see cref="KisIdentity"/> object.
        /// </summary>
        /// <param name="refreshToken">A KIS refresh token.</param>
        /// <remarks>
        /// If KIS returns 403 Forbidden (login not allowed), it means that the user has lost their membership.
        /// From this system's perspective, it means that the user doesn't exist anymore so an empty object is
        /// returned to maintain consistency with <see cref="GetIdentityFromSession"/>.
        /// The returned object may be cached.
        /// </remarks>
        /// <returns>A <see cref="KisIdentity"/> object with the KIS tokens and user data.
        /// If KIS returns 403 Forbidden or 404 Not Found, the object has all its properties set to null.
        /// If another error occurs, null is returned.
        /// </returns>
        Task<KisIdentity> GetIdentityFromRefreshToken(string refreshToken);

        /// <summary>
        /// Fetches current tap info from KIS for the configured tap IDs.
        /// </summary>
        /// <returns>A collection of <see cref="KisTapInfo"/> objects with information about the taps or null
        /// when no tap data can be fetched.
        /// The <see cref="KisTapInfo.OfferedArticles"/> property of the returned objects is null if no drink
        /// is currently associated with the tap.</returns>
        Task<ICollection<KisTapInfo>> GetTapInfo();

        /// <summary>
        /// Fetches offered articles info from KIS.
        /// </summary>
        /// <remarks>
        /// Articles that aren't in stock may be returned as well. Check <see cref="KisArticle.InStock"/> for more info.
        /// </remarks>
        /// <returns>A collection of <see cref="KisArticle"/> objects with information about the articles in KIS
        /// or null if an error occurs.</returns>
        Task<ICollection<KisArticle>> GetOfferedArticles();

        /// <summary>
        /// Fetches a list of users with the largest prestige gain in a given time period.
        /// </summary>
        /// <param name="from">Only includes prestige gains that occurred after this time.</param>
        /// <param name="to">Only includes prestige gains that occurred before this time.</param>
        /// <param name="count">Number of entries to return. Must be more than 0 and less than 100.</param>
        /// <returns>A list of <see cref="KisLeaderboardItem"/> objects with information about the top
        /// <paramref name="count"/> prestige gaining users in the given time period
        /// or null if an error occurs.</returns>
        Task<IList<KisLeaderboardItem>> GetLeaderboard(DateTime from, DateTime to, int count);
    }
}
