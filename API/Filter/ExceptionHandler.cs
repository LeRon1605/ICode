using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Filter
{
    public class ExceptionHandler: ExceptionFilterAttribute
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger _logger;

        public ExceptionHandler(IHostEnvironment hostEnvironment, ILoggerFactory factory)
        {
            _hostEnvironment = hostEnvironment;
            _logger = factory.CreateLogger("ExeptionFiler");
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Error At {Time}", DateTime.UtcNow);
            if (!_hostEnvironment.IsDevelopment())
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Title = "An error occured while processing your request.",
                    Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
                    Instance = context.HttpContext.Request.Path,
                    Status = 500
                });
                return;
            }
            var res = new ProblemDetails
            {
                Title = "An error occured while processing your request.",
                Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
                Instance = context.HttpContext.Request.Path,
                Status = 500,
                Detail = context.Exception.Message,
            };
            res.Extensions.Add("StackTrace", context.Exception.StackTrace);
            context.Result = new ObjectResult(res);
        }
    }
}
