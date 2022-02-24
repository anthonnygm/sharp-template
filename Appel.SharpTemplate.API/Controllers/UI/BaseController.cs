using Microsoft.AspNetCore.Mvc;

namespace Appel.SharpTemplate.API.Controllers.UI;

public class BaseController : Controller
{
    protected IActionResult ViewError(string message)
    {
        ViewBag.ErrorMessage = message;

        return View("Error");
    }
}
