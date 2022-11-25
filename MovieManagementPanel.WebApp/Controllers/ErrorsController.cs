using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieManagementPanel.WebApp.Models;

namespace MovieManagementPanel.WebApp.Controllers
{
    [AllowAnonymous]
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [Route("/error-development")]
        public IActionResult ErrorDevelopment()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError(exceptionDetails?.Error, exceptionDetails?.Error.Message);

            ErrorViewModel errorViewModel = new()
            {
                ExceptionMessage = exceptionDetails?.Error.Message ?? "",
                ExceptionPath = exceptionDetails?.Path ?? "",
                ExceptionStackTrace = exceptionDetails?.Error.StackTrace ?? ""
            };

            return View(errorViewModel);
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError(exceptionDetails?.Error, exceptionDetails?.Error.Message);

            return View();
        }

        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("/unauthorized")]
        public IActionResult UnAuthorized()
        {
            return View();
        }
    }
}
