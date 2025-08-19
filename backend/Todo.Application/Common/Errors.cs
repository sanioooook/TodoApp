namespace Todo.Application.Common;

using FluentResults;

public static class Errors
{
    public static Error NotFound(string entity, Guid id) =>
        new Error($"{entity} with id {id} not found")
            .WithMetadata("ErrorCode", "NotFound");

    public static Error Validation(string message) =>
        new Error(message)
            .WithMetadata("ErrorCode", "Validation");

    public static Error Unauthorized(string message) =>
        new Error(message)
            .WithMetadata("ErrorCode", "Unauthorized");

    public static Error Conflict(string message) =>
        new Error(message)
            .WithMetadata("ErrorCode", "Conflict");
}