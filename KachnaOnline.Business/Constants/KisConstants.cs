// KisConstants.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Constants
{
    public class KisConstants
    {
        // Name of the named HTTP client for accessing KIS without authorization
        public const string KisHttpClient = "kis";
        // Name of the named HTTP client for accessing KIS using the configured display token
        public const string KisDisplayHttpClient = "kis_display";
    }
}
