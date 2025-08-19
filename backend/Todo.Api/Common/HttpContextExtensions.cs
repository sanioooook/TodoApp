namespace Todo.Api.Common;

public static class HttpContextExtensions
{
    public static Guid GetCurrentUserId(this HttpContext context)
    {
        if (context.Items.TryGetValue("CurrentUserId", out var value) && value is Guid userId)
            return userId;

        throw new InvalidOperationException("Current user id not found");
    }
}