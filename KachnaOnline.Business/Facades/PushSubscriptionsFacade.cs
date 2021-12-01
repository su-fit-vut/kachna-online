// PushSubscriptionsFacade.cs
// Author: František Nečas

using KachnaOnline.Business.Configuration;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Facades
{
    public class PushSubscriptionsFacade
    {
        private readonly IOptionsMonitor<PushOptions> _pushOptions;

        public PushSubscriptionsFacade(IOptionsMonitor<PushOptions> pushOptions)
        {
            _pushOptions = pushOptions;
        }

        public string GetPublicKey()
        {
            return _pushOptions.CurrentValue.PublicKey;
        }
    }
}
