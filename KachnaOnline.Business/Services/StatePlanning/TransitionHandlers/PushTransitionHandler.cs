// PushTransitionHandler.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.Push;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class PushTransitionHandler : PushNotificationClient, IStateTransitionHandler
    {
        private readonly IPushSubscriptionsService _pushSubscriptionsService;

        public PushTransitionHandler(IOptionsMonitor<PushOptions> pushOptions, PushServiceClient pushClient,
            IPushSubscriptionsService pushSubscriptionsService) : base(pushOptions, pushClient)
        {
            _pushSubscriptionsService = pushSubscriptionsService;
        }

        public async Task PerformStartAction(int stateId, int? previousStateId)
        {
            // TODO: add a better message.
            foreach (var subscription in await _pushSubscriptionsService.GetStateChangesEnabledSubscriptions())
            {
                await this.SendNotification(subscription, "Otviracka", "Otvirame kachnu", "");
            }
        }

        public Task PerformEndAction(int stateId, int? nextStateId)
        {
            return Task.CompletedTask;
        }
    }
}
