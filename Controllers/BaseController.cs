using Inno.Services;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IActionResult AjaxSuccess(object data = null)
            => Ok(new { success = true, data = data });

        protected IActionResult AjaxFail(string message)
            => Ok(new { success = false, error = message });


        protected OkObjectResult GetModelError()
        {
            if (ModelState.ErrorCount > 0)
            {
                // 1. جمع‌آوری تمام خطاهای
                var modelErrors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Key = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                // 2. ساختن یک رشته واحد از تمام خطاها، هر خطا در یک خط
                var aggregatedErrorMessage = modelErrors
                    .SelectMany(me => me.Errors.Select(err => $"{me.Key} {err}"))
                    .Aggregate((current, next) => current + "\n" + next);

                return Ok(new
                {
                    success = false,
                    error = aggregatedErrorMessage
                });
            }
            return Ok(new { success = true });
        }
    }
}