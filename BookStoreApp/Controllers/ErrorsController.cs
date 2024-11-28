using Microsoft.AspNetCore.Mvc;

public class ErrorsController : Controller
{
    [Route("Errors/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        if (statusCode == 404)
        {
            return View("404");
        }
        if (statusCode == 500)
        {
            return View("500");
        }
        return View("Error");
    }
}
