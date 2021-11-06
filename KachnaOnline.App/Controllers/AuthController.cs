// AuthController.cs
// Author: Ondřej Ondryáš

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Exchanges a KIS eduID session ID for a local access token.
        /// </summary>
        /// <remarks>
        /// A KIS eduID session ID may be obtained by making a GET request to KIS /auth/eduid.
        /// A possible login flow is that a frontend calls KIS /auth/eduid and provides a URL pointing to this
        /// endpoint as the 'redirect' parameter. KIS returns an eduID redirect link to which the frontend redirects
        /// the user. After logging in, the user gets redirected by KIS to this endpoint with the <paramref name="session"/>
        /// parameter set. This endpoint will then provide the user with an access token to this service's API.
        /// </remarks>
        /// <param name="session">A KIS eduID session ID.</param>
        /// <returns>A JWT Bearer token representing the user's identity.</returns>
        /// <response code="200">A JWT Bearer token representing the user's identity.</response>
        /// <response code="400">No session ID was provided.</response>
        /// <response code="404">User is not registered in KIS.</response>
        [HttpGet("accessTokenFromSession")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> LoginSession(
            [Required(ErrorMessage = "A KIS session ID must be provided.")] [FromQuery]
            string session)
        {
            var token = await _userService.LoginSession(session);
            if (token.HasError)
            {
                return this.Problem(statusCode: 500, detail: "An unexpected error occured.");
            }
            else if (!token.UserFound)
            {
                return this.Problem(statusCode: 404, detail: "User is not registered in KIS.");
            }

            return this.Ok(token.AccessToken);
        }

        /// <summary>
        /// Exchanges a KIS refresh token for a local access token.
        /// </summary>
        /// <remarks>
        /// This may be used to log in to the application directly from another KIS application,
        /// such as from the administration or the operator app.
        /// </remarks>
        /// <param name="kisRefreshToken">A KIS refresh token.</param>
        /// <returns>A JWT Bearer token representing the user's identity.</returns>
        /// <response code="200">A JWT Bearer token representing the user's identity.</response>
        /// <response code="400">No refresh token was provided.</response>
        /// <response code="403">Invalid refresh token (either it has expired or the user is not an SU member anymore).</response>
        [HttpGet("accessTokenFromRefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<string>> LoginToken(
            [Required(ErrorMessage = "A KIS refresh token must be provided.")] [FromQuery] string kisRefreshToken)
        {
            var token = await _userService.LoginToken(kisRefreshToken);
            if (token.HasError)
            {
                return this.Problem(statusCode: 500, detail: "An unexpected error occured.");
            }
            else if (!token.UserFound)
            {
                return this.Problem(statusCode: 403,
                    detail:
                    "Invalid KIS refresh token (either it has expired or the user is not an SU member anymore).");
            }

            return this.Ok(token.AccessToken);
        }

        /// <summary>
        /// Exchanges a KIS refresh token for a local access token. The refresh token is obtained from
        /// a provided access token.
        /// </summary>
        /// <remarks>
        /// A JWT access token issued by this API contains a KIS refresh token for the target user.
        /// This token is then used as if it was provided to the accessTokenFromRefreshToken endpoint.
        /// </remarks>
        /// <returns>A JWT Bearer token representing the user's identity.</returns>
        /// <response code="200">A JWT Bearer token representing the user's identity.</response>
        /// <response code="403">Invalid refresh token (either it has expired or the user is not an SU member anymore).</response>
        [HttpGet("refreshedAccessToken")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<string>> Refresh()
        {
            var token = await _userService.Refresh(this.User);
            if (token.HasError)
            {
                return this.Problem(statusCode: 500, detail: "An unexpected error occured.");
            }
            else if (!token.UserFound)
            {
                return this.Problem(statusCode: 403,
                    detail:
                    "Invalid KIS refresh token (either it has expired or the user is not an SU member anymore).");
            }

            return this.Ok(token.AccessToken);
        }
    }
}
