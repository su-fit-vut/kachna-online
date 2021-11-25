// ControllerBaseExtensions.cs
// Author: Ondřej Ondryáš

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ObjectResult NotFoundProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status404NotFound, title);
        }

        public static ObjectResult ConflictProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status409Conflict, title);
        }

        public static ObjectResult BadRequestProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status400BadRequest, title);
        }

        public static ObjectResult ForbiddenProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status403Forbidden, title);
        }

        public static ObjectResult UnauthorizedProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status401Unauthorized, title);
        }

        public static ObjectResult UnprocessableEntityProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message, null, StatusCodes.Status422UnprocessableEntity, title);
        }

        public static ObjectResult GeneralProblem(this ControllerBase controller, string message = null,
            string title = null)
        {
            return controller.Problem(message ?? "An unexpected error occured", null,
                StatusCodes.Status500InternalServerError, title);
        }
    }
}
