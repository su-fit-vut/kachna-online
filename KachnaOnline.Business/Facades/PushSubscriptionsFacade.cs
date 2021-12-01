// PushSubscriptionsFacade.cs
// Author: František Nečas

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Dto.PushNotifications;
using Microsoft.Extensions.Options;
using KachnaOnline.Business.Exceptions.PushNotifications;
using KachnaOnline.Business.Models.PushNotifications;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Constants;

namespace KachnaOnline.Business.Facades
{
    public class PushSubscriptionsFacade
    {
        private readonly IOptionsMonitor<PushOptions> _pushOptions;
        private readonly IMapper _mapper;
        private readonly IPushSubscriptionsService _pushSubscriptionsService;

        public PushSubscriptionsFacade(IOptionsMonitor<PushOptions> pushOptions, IMapper mapper,
            IPushSubscriptionsService pushSubscriptionsService)
        {
            _pushOptions = pushOptions;
            _mapper = mapper;
            _pushSubscriptionsService = pushSubscriptionsService;
        }

        /// <summary>
        /// Returns a public VAPID key.
        /// </summary>
        /// <returns>The public VAPID key.</returns>
        public string GetPublicKey()
        {
            return _pushOptions.CurrentValue.PublicKey;
        }

        /// <summary>
        /// Subscribes a user to push notifications.
        /// </summary>
        /// <param name="user">Identity of the user to subscribe.</param>
        /// <param name="subscription">Configuration of the subscription.</param>
        /// <exception cref="PushNotificationManipulationFailedException">When the subscription failed.</exception>
        public async Task Subscribe(ClaimsPrincipal user, PushSubscriptionDto subscription)
        {
            var subscriptionModel = _mapper.Map<PushSubscription>(subscription);
            try
            {
                subscriptionModel.MadeById = int.Parse(user.FindFirstValue(IdentityConstants.IdClaim));
            }
            catch (ArgumentException)
            {
                subscriptionModel.MadeById = null;
            }
            await _pushSubscriptionsService.CreateOrUpdateSubscription(subscriptionModel);
        }

        /// <summary>
        /// Unsubscribes a user from push notifications.
        /// </summary>
        /// <param name="endpoint">Endpoint to stop sending push notifications to.</param>
        public async Task Unsubscribe(string endpoint)
        {
            await _pushSubscriptionsService.DeletePushSubscription(endpoint);
        }

        /// <summary>
        /// Returns subscription configuration of the given endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint to get the configuration of.</param>
        /// <returns>The configuration of the <paramref name="endpoint"/>. Null if no configuration is present.</returns>
        public async Task<PushSubscriptionConfiguration> GetSubscription(string endpoint)
        {
            var subscription = await _pushSubscriptionsService.GetPushSubscription(endpoint);
            if (subscription == null)
            {
                return null;
            }

            return _mapper.Map<PushSubscriptionConfiguration>(subscription);
        }
    }
}
