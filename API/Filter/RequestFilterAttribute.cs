using API.Models.DTO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace API.Filter
{
    public class RequestFilterAttribute: ActionFilterAttribute
    {
        private readonly ILogger<RequestFilterAttribute> _logger;
        public RequestFilterAttribute(ILogger<RequestFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            LogRequest logRequest = new LogRequest
            {
                AccessIP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                AccessTime = DateTime.Now,
                UserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString(),
                Path = context.HttpContext.Request.Path,
                Controller = context.HttpContext.Request.RouteValues["controller"].ToString(),
                Method = context.HttpContext.Request.Method,
                UserId = context.HttpContext.User.FindFirst("ID")?.Value,
                Action = context.HttpContext.Request.RouteValues["action"].ToString(),
                ActionArguments = context.ActionArguments 
            };

            _logger.LogInformation(logRequest.ToString());
            
        }
    }
}
