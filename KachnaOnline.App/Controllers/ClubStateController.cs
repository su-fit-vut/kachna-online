// ClubStateController.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubStateController : ControllerBase
    {
        private readonly ILogger<ClubStateController> _logger;

        public ClubStateController(ILogger<ClubStateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Stub";
        }
    }
}
