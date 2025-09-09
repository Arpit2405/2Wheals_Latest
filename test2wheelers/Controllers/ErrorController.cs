using Microsoft.AspNetCore.Mvc;

namespace _2whealers.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("PageNotFound");   // Views/Error/PageNotFound.cshtml
                case 500:
                    return View("InternalServerError"); // Views/Error/InternalServerError.cshtml
                default:
                    return View("Error"); // Generic fallback
            }
        }

        [Route("Error/500")]
        public IActionResult InternalServerError()
        {
            return View("InternalServerError");
        }
    }
}
