// IdentityConstants.cs
// Author: Ondřej Ondryáš

using Microsoft.IdentityModel.JsonWebTokens;

namespace KachnaOnline.Business.Constants
{
    public static class IdentityConstants
    {
        public const string IdClaim = JwtRegisteredClaimNames.Sub;
        public const string EmailClaim = JwtRegisteredClaimNames.Email;
        public const string NameClaim = JwtRegisteredClaimNames.GivenName;
        public const string RolesClaim = "role";
        public const string KisRefreshTokenClaim = "krt";
        public const string KisNicknameClaim = "knick";
    }
}
