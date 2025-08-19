namespace Todo.Api.Common;

using FluentResults;
using Microsoft.AspNetCore.Mvc;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return MapErrors(result.Errors);
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return MapErrors(result.Errors);
    }

    private static IActionResult MapErrors(IReadOnlyList<IError> errors)
    {
        // grouping errors by ErrorCode
        var grouped = errors
            .GroupBy(e => e.Metadata.TryGetValue("ErrorCode", out var code) ? code?.ToString() : "Unknown")
            .ToDictionary(g => g.Key!, g => g.Select(e => e.Message).ToList());

        // priority: Validation → NotFound → Unauthorized → Conflict → Unknown → 500
        if (grouped.ContainsKey("Validation"))
            return new BadRequestObjectResult(grouped);

        if (grouped.ContainsKey("NotFound"))
            return new NotFoundObjectResult(grouped);

        if (grouped.ContainsKey("Unauthorized"))
            return new UnauthorizedObjectResult(grouped);

        if (grouped.ContainsKey("Conflict"))
            return new ConflictObjectResult(grouped);

        // if code not set — InternalServerError
        return new ObjectResult(grouped) { StatusCode = 500 };
    }
}