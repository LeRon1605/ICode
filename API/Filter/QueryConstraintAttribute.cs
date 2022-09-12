using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DTO;
using System.Linq;

namespace API.Filter
{
    public class QueryConstraintAttribute : ActionFilterAttribute
    {
        public string Key { get; set; }
        public string Depend { get; set; } = null;
        public string Value { get; set; } = null;
        public bool Retrict { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Depend != null)
            {
                if (!context.ActionArguments.ContainsKey(Depend))
                {
                    return;
                }
            }
            if (context.ActionArguments.ContainsKey(Key))
            {
                if (Value != null)
                {
                    if (Value.Split(",").FirstOrDefault(x => x.Trim() == (string)context.ActionArguments[Key]) == null)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            error = "Invalid Action.",
                            detail = $"The value '{(string)context.ActionArguments[Key]}' is not acceptable for {Key} field"
                        });
                        return;
                    }
                }
            }
            else
            {
                if (Retrict)
                {
                    context.Result = new BadRequestObjectResult(new ErrorResponse
                    {
                        error = "Invalid Action",
                        detail = $"The request is required to provide '{Key}' field."
                    });
                    return;
                }
            }  
        }
    }
}
