﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Filter
{
    public class ValidationModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                Dictionary<string, string[]> list = new Dictionary<string, string[]>();
                foreach (var item in context.ModelState)
                {
                    if (item.Value.ValidationState == ModelValidationState.Invalid)
                    {
                        list.Add(item.Key, item.Value.Errors.Select(x => x.ErrorMessage).ToArray());
                    }
                }

                context.Result = new BadRequestObjectResult(new
                {
                    Title = "Validation error.",
                    Instance = context.HttpContext.Request.Path.ToString(),
                    Status = 400,
                    Detail = "One or more validation error.",
                    Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
                    ModelState = list
                });
                return;
            }
        }
    }
}
