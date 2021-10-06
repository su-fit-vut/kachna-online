// JwtOptions.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Configuration
{
    public class JwtOptions
    {
        /// <summary>
        /// A secret key that is used to sign the issued JWTs.
        /// </summary>
        public string Secret { get; set; }
        
        /// <summary>
        /// Validity (time between issuing and expiration) of issued JWTs in seconds.
        /// </summary>
        /// <remarks>
        /// This should be set to a value lower than the validity of KIS refresh token to enable
        /// identity refreshing. KIS refresh tokens are currently valid for 60 minutes and their
        /// validity is prolonged when the token is used.
        /// </remarks>
        public int ValiditySeconds { get; set; }
    }
}
