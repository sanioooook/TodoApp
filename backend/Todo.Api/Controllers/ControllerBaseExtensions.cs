namespace Todo.Api.Controllers;

using Application.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    public static async Task<ActionResult?> ValidateAndReturnIfInvalid<T>(
        this ControllerBase controller,
        IValidator<T> validator,
        T model)
    {
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return controller.ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary())
            {
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = controller.HttpContext.Request.Path
            });
        }

        return null; // validation is passed
    }

    public static IActionResult HandleNonSuccess(this ControllerBase controller, Todo.Application.Interfaces.IResult result)
    {
        return result.CodeResult switch
        {
            ResultCode.BadRequest => controller.BadRequest(result.Message),
            ResultCode.Forbidden => controller.Forbid(result.Message),
            ResultCode.NotFound => controller.NotFound(result.Message),
            ResultCode.Conflict => controller.Conflict(result.Message),
            ResultCode.ServerError => controller.Problem(title: result.Message, statusCode: 500),
            _ => controller.Ok(result.Message) // fallback
        };
    }
}