// LoginResult.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Models.Users
{
    public struct LoginResult
    {
        /// <summary>
        /// A signed JWT bearing an authenticated user's identity.
        /// </summary>
        public string AccessToken;
        
        /// <summary>
        /// If false, the user isn't registered in KIS, the provided KIS refresh token
        /// has expired or the user isn't an SU member anymore.
        /// </summary>
        public bool UserFound;
        
        /// <summary>
        /// If true, an exception occured when requesting user data from KIS or synchronizing
        /// the user.
        /// </summary>
        public bool HasError;
    }
}
