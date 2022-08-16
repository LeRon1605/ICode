using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Filter
{
    public class ExceptionHandler: IExceptionFilter
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ExceptionHandler(IHostEnvironment hostEnvironment) =>
            _hostEnvironment = hostEnvironment;

        public void OnException(ExceptionContext context)
        {
            if (!_hostEnvironment.IsDevelopment())
            {
                return;
            }
            var result = new JsonResult(new
            {
                error = "Exception.",
                detail = new 
                {
                    message = context.Exception.Message,
                    stack = context.Exception.StackTrace
                }
            });
            result.StatusCode = 500;
            context.Result = result;
        }
    }
}
