// ClubStateController.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubStates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("states")]
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
        public async Task<StateDto> Get()
        {
            return null;
        }
    }
}
