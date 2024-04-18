using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using WebApi.Contracts;

namespace WebApi.Filter
{
    public class ValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var api_error = new ErrorResponse
                {
                    StatusCode = 400,
                    ErrorMessage = "Bad Request",
                    Timestamp = DateTime.Now
                };

                var errors = context.ModelState.AsEnumerable();

                foreach (var error in errors)
                {
                    foreach (var inner in error.Value!.Errors)
                    {
                        api_error.Errors.Add(inner.ErrorMessage);
                    }
                }

                context.Result = new BadRequestObjectResult(JsonSerializer.Serialize(new { errors = api_error }));
            }
        }
    }
}
