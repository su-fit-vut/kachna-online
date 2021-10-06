// ClubStateController.cs
// Author: Ondřej Ondryáš

using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubStateController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<ClubStateController> _logger;

        public ClubStateController(IUserService userService,
            ILogger<ClubStateController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Stub";
        }

        [HttpGet("Test")]
        [Authorize]
        public async Task<object> Test()
        {
            return new
            {
                LocalUser = await _userService.GetUser(this.User),
                RolesString = await _userService.GetUserRoles(this.User),
                Roles = await _userService.GetUserRoleDetails(this.User),
                Claims = this.User.Claims.Select(c => new {c.Type, c.Value})
            };
        }
    }
}
