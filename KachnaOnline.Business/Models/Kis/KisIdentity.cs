// KisIdentity.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.Kis
{
    public class KisIdentity
    {
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AuthTokenExpiry { get; set; }
        public KisUser UserData { get; set; }
    }
}
