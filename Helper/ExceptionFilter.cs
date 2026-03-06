using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inno.Helper
{
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

            //خطاهای بیزینسی
            if (context.Exception is SysException sysEx)
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    error = sysEx.Message
                })
                { StatusCode = 200 };

                context.ExceptionHandled = true;
                return;
            }

            //سایر خطاها
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
                { StatusCode = 200 };
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    error = "Server Error!"
                })
                { StatusCode = 200 };
            }

            context.ExceptionHandled = true;
        }
    }
}