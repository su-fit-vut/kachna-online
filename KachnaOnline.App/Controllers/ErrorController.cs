using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("{code?}")]
        [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        public IActionResult Get(int? code)
        {
            return this.Problem(statusCode: code);
        }
    }
}
