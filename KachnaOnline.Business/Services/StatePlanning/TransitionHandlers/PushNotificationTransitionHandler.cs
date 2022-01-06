// PushTransitionHandler.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.Push;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class PushNotificationTransitionHandler : PushNotificationClient, IStateTransitionHandler
    {
        private readonly IPushSubscriptionsService _pushSubscriptionsService;
        private readonly IClubStateService _stateService;
        private readonly ILogger<PushNotificationTransitionHandler> _logger;

        public PushNotificationTransitionHandler(IOptionsMonitor<PushOptions> pushOptions, PushServiceClient pushClient,
            IPushSubscriptionsService pushSubscriptionsService, IClubStateService stateService,
            ILogger<PushNotificationTransitionHandler> logger) : base(pushOptions, pushClient, logger)
        {
            _pushSubscriptionsService = pushSubscriptionsService;
            _stateService = stateService;
            _logger = logger;
        }

        public async Task PerformStartAction(int stateId, int? previousStateId)
        {
            var state = await _stateService.GetState(stateId);
            if (state is null)
            {
                _logger.LogCritical("State {Id} that triggered transition handlers does not exist.", stateId);
                return;
            }

            // Closed should never occur here but whatever
            if (state.Type is StateType.Private or StateType.Closed)
                return;

            _logger.LogDebug("Sending notifications for start of state {Id}.", stateId);
            foreach (var subscription in await _pushSubscriptionsService.GetStateChangesEnabledSubscriptions())
            {
                var title = state.Type == StateType.OpenBar ? "Otvíračka v Kachně" : "Chillzóna v Kachně";
                var msg = state.Type == StateType.OpenBar
                    ? $"U Kachničky je otevřeno, bar je obsluhován, pípy naraženy, tak se stav! Končíme ve {state.PlannedEnd:HH:mm}."
                    : $"U Kachničky je otevřeno v režimu chillzóna do {state.PlannedEnd:HH:mm}.";

                try
                {
                    await this.SendNotification(subscription, title, msg);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot send a push notification.");
                }
            }
        }

        public Task PerformModifyAction(State previousState)
        {
            return Task.CompletedTask;
        }

        public Task PerformEndAction(int stateId, int? nextStateId)
        {
            return Task.CompletedTask;
        }
    }
}
