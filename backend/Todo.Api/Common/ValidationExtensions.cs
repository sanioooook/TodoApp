namespace Todo.Api.Common;

using FluentResults;
using FluentValidation;

public static class ValidationExtensions
{
    public static async Task<Result> ValidateWithResultAsync<T>(
        this IValidator<T> validator,
        T model,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (validationResult.IsValid)
            return Result.Ok();

        var errors = validationResult.Errors
            .Select(e => new Error(e.ErrorMessage)
                .WithMetadata("ErrorCode", "Validation")
                .WithMetadata("PropertyName", e.PropertyName))
            .ToList();

        return Result.Fail(errors);
    }
}