using API.Models.Data;
using API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Filter
{
    public class ValidateIDAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("ID"))
            {
                if (string.IsNullOrEmpty((string)context.ActionArguments["ID"]))
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Invalid ID"
                    });
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(new
                {
                    status = false,
                    message = "ID Required"
                });
                return;
            }  
        }
    }
}
