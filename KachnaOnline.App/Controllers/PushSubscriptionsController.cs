// PushSubscriptionsController.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Facades;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("push")]
    public class PushSubscriptionsController
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
        [HttpGet("publicKey")]
        public ActionResult<string> GetPublicKey()
        {
            return new ActionResult<string>(_facade.GetPublicKey());
        }
    }
}
