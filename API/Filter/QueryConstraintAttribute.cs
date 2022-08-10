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
    public class QueryConstraintAttribute : ActionFilterAttribute
    {
        public string Key { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey(Key))
            {
                if (string.IsNullOrEmpty((string)context.ActionArguments[Key]))
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        message = $"Invalid {Key}"
                    });
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(new
                {
                    message = $"{Key} Required"
                });
                return;
            }  
        }
    }
}
