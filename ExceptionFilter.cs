using Inno;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionFilter(ILogger<ExceptionFilter> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled Exception");

        // خطاهای بیزینسی/سیستمی خودت (هم Dev هم Prod)
        if (context.Exception is SysException sysEx)
        {
            context.Result = new JsonResult(new
            {
                success = false,
                error = sysEx.Message,

                // فقط در Dev
                detail = _env.IsDevelopment() ? new
                {
                    type = context.Exception.GetType().FullName,
                    message = context.Exception.Message,
                    stackTrace = context.Exception.StackTrace,
                    inner = context.Exception.InnerException?.ToString()
                } : null
            })
            { StatusCode = 400 };

            context.ExceptionHandled = true;
            return;
        }

        // سایر Exceptionها
        if (_env.IsDevelopment())
        {
            var ex = context.Exception;

            context.Result = new JsonResult(new
            {
                success = false,
                error = ex.Message,
                detail = new
                {
                    type = ex.GetType().FullName,
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    inner = ex.InnerException?.ToString()
                }
            })
            { StatusCode = 500 };
        }
        else
        {
            context.Result = new JsonResult(new
            {
                success = false,
                error = "Server Error!"
            })
            { StatusCode = 500 };
        }

        context.ExceptionHandled = true;
    }
}