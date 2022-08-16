using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
                        message = $"Invalid {Key}."
                    });
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(new
                {
                    message = $"The request is required to provide {Key} query."
                });
                return;
            }  
        }
    }
}
