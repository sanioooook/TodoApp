namespace Todo.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CurrentUserFilter : IAsyncActionFilter
{
    private const string HeaderName = "X-User-Id";
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var headerValue) ||
            !Guid.TryParse(headerValue, out var userId))
        {
            context.Result = new BadRequestObjectResult($"{HeaderName} header is missing or invalid.");
            return;
        }

        context.HttpContext.Items["CurrentUserId"] = userId;

        await next();
    }
}