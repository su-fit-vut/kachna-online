// PushSubscriptionsController.cs
// Author: František Nečas

using System;
using System.Net;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Exceptions.PushNotifications;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.PushNotifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("push")]
    public class PushSubscriptionsController : ControllerBase
    {
        private readonly PushSubscriptionsFacade _facade;

        public PushSubscriptionsController(PushSubscriptionsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns the public VAPID key.
        /// </summary>
        /// <response code="200">The public VAPID key of the server.</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpGet("publicKey")]
        public ActionResult<string> GetPublicKey()
        {
            return new ActionResult<string>(_facade.GetPublicKey());
        }

        /// <summary>
        /// Creates a new push subscription with the given configuration.
        /// </summary>
        /// <remarks>
        /// This is implemented as a PUT endpoint in order to support cases such as a user subscribing as
        /// an anonymous user and later logging in to subscribe as a logged in user (e.g. for board game reservations).
        /// Hence this endpoint updates existing configuration if it exists, or creates a new one if it does not.
        /// </remarks>
        /// <param name="subscription">Configuration of the subscription.</param>
        /// <response code="204">The subscription was created.</response>
        /// <response code="401">When a subscription which only logged-in user can use was requested.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("subscriptions")]
        public async Task<IActionResult> Subscribe(PushSubscriptionDto subscription)
        {
            if ((!this.User.Identity?.IsAuthenticated ?? false) &&
                (subscription.Configuration.BoardGamesEnabled ?? false))
            {
                return this.UnauthorizedProblem(
                    "Board games reservation subscriptions can only be enabled by an authenticated user.");
            }

            await _facade.Subscribe(this.User, subscription);
            return this.NoContent();
        }

        /// <summary>
        /// Cancels a push subscription.
        /// </summary>
        /// <param name="endpoint">Endpoint of the active push subscription to delete.</param>
        /// <response code="204">The subscription has been cancelled.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("subscriptions/{endpoint}")]
        public async Task<IActionResult> Unsubscribe(string endpoint)
        {
            await _facade.Unsubscribe(WebUtility.UrlDecode(endpoint));
            return this.NoContent();
        }

        /// <summary>
        /// Gets configuration of an existing subscription.
        /// </summary>
        /// <param name="endpoint">The endpoint of an active push subscription to get configuration of.</param>
        /// <response code="200">The configuration.</response>
        /// <response code="404">No such subscription exists.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("subscriptions/{endpoint}")]
        public async Task<ActionResult<PushSubscriptionConfiguration>> GetConfiguration(string endpoint)
        {
            var subscription = await _facade.GetSubscription(WebUtility.UrlDecode(endpoint));
            if (subscription == null)
            {
                return this.NotFoundProblem("No such subscription exists.");
            }

            return new ActionResult<PushSubscriptionConfiguration>(subscription);
        }
    }
}
