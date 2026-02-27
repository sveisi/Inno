using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : Controller
{
    protected IActionResult AjaxSuccess()
        => Ok(new { success = true });

    protected IActionResult AjaxFail(string message)
        => Ok(new { success = false, error = message });
}