// AuthenticationResultDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.Auth
{
    public class AuthenticationResultDto
    {
        /// <summary>
        /// A signed JWT bearing the authenticated user's identity.
        /// </summary>
        public string AccessToken;

        /// <summary>
        /// A KIS access token for the authenticated user.
        /// </summary>
        public string KisAccessToken;
    }
}
