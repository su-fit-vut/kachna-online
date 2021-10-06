// IKisService.cs
// Author: Ondřej Ondryáš

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
        /// </remarks>
        /// <returns>A <see cref="KisIdentity"/> object with the KIS tokens and user data.
        /// If KIS returns 403 Forbidden or 404 Not Found, the object has all its properties set to null.
        /// If another error occurs, null is returned.
        /// </returns>
        Task<KisIdentity> GetIdentityFromRefreshToken(string refreshToken);
    }
}
