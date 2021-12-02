// PushSubscriptionsService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions.PushNotifications;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Data.Entities.PushSubscriptions;
using Microsoft.Extensions.Logging;
using PushSubscription = KachnaOnline.Business.Models.PushNotifications.PushSubscription;
using PushSubscriptionEntity = KachnaOnline.Data.Entities.PushSubscriptions.PushSubscription;

namespace KachnaOnline.Business.Services
{
    public class PushSubscriptionsService : IPushSubscriptionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushSubscriptionsRepository _pushSubscriptions;
        private readonly ILogger<PushSubscriptionsService> _logger;
        private readonly IMapper _mapper;

        public PushSubscriptionsService(IUnitOfWork unitOfWork, ILogger<PushSubscriptionsService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _pushSubscriptions = _unitOfWork.PushSubscriptions;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task CreateOrUpdateSubscription(PushSubscription subscription)
        {
            var sub = await _pushSubscriptions.GetWithKeys(subscription.Endpoint);
            if (sub is null)
            {
                var entity =
                    _mapper.Map<KachnaOnline.Data.Entities.PushSubscriptions.PushSubscription>(subscription);
                entity.Keys = new List<PushSubscriptionKey>();
                foreach (var (key, value) in subscription.Keys)
                {
                    entity.Keys.Add(new PushSubscriptionKey()
                    {
                        Endpoint = subscription.Endpoint,
                        KeyType = key,
                        KeyValue = value
                    });
                }

                await _pushSubscriptions.Add(entity);
            }
            else
            {
                if (sub.MadeById == null)
                {
                    sub.MadeById = subscription.MadeById;
                }

                if (subscription.BoardGamesEnabled.HasValue)
                {
                    sub.BoardGamesEnabled = subscription.BoardGamesEnabled.Value;
                }

                sub.StateChangesEnabled = subscription.StateChangesEnabled;
                // Also update keys
                foreach (var (key, value) in subscription.Keys)
                {
                    var hasKey = false;
                    foreach (var existingKey in sub.Keys)
                    {
                        if (existingKey.KeyType == key)
                        {
                            existingKey.KeyValue = value;
                            hasKey = true;
                        }
                    }

                    // New key
                    if (!hasKey)
                    {
                        sub.Keys.Add(new PushSubscriptionKey()
                        {
                            Endpoint = sub.Endpoint,
                            KeyType = key,
                            KeyValue = value
                        });
                    }
                }
            }

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create or update subscription.");
                await _unitOfWork.ClearTrackedChanges();
                throw new PushNotificationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task DeletePushSubscription(string endpoint)
        {
            var entity = await _pushSubscriptions.Get(endpoint);
            try
            {
                if (entity != null)
                {
                    await _pushSubscriptions.Delete(entity);
                }

                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete push subscription.");
                await _unitOfWork.ClearTrackedChanges();
                throw new PushNotificationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task<PushSubscription> GetPushSubscription(string endpoint)
        {
            return _mapper.Map<PushSubscription>(await _pushSubscriptions.Get(endpoint));
        }

        private void MapKeys(PushSubscription target, PushSubscriptionEntity source)
        {
            target.Keys = new Dictionary<string, string>();
            foreach (var key in source.Keys)
            {
                target.Keys.Add(key.KeyType, key.KeyValue);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<PushSubscription>> GetStateChangesEnabledSubscriptions()
        {
            var subscriptions = new List<PushSubscription>();
            await foreach (var subscription in _pushSubscriptions.GetSubscribedToStateChanges())
            {
                var model = _mapper.Map<PushSubscription>(subscription);
                this.MapKeys(model, subscription);
                subscriptions.Add(model);
            }

            return subscriptions;
        }

        /// <inheritdoc />
        public async Task<ICollection<PushSubscription>> GetUserBoardGamesEnabledSubscriptions(int userId)
        {
            var subscriptions = new List<PushSubscription>();
            await foreach (var subscription in _pushSubscriptions.GetUserBoardGamesSubscription(userId))
            {
                var model = _mapper.Map<PushSubscription>(subscription);
                this.MapKeys(model, subscription);
                subscriptions.Add(model);
            }

            return subscriptions;
        }
    }
}
