using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.DeleteTodoList;

public class DeleteTodoListValidatorTests
{
    private readonly DeleteTodoListValidator _validator;

    public DeleteTodoListValidatorTests()
    {
        _validator = new DeleteTodoListValidator();
    }

    [Fact]
    public void DeleteTodoListValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new DeleteTodoListCommand
        {
            Id = Guid.NewGuid(),
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeleteTodoListValidator_EmptyCurrentUserId_FailsValidation()
    {
        // Arrange
        var command = new DeleteTodoListCommand
        {
            Id = Guid.NewGuid(),
            CurrentUserId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentUserId)
              .WithErrorMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");
    }

    [Fact]
    public void DeleteTodoListValidator_EmptyId_FailsValidation()
    {
        // Arrange
        var command = new DeleteTodoListCommand
        {
            Id = Guid.Empty,
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id is required");
    }

    [Fact]
    public void DeleteTodoListValidator_NullId_FailsValidation()
    {
        // Arrange
        var command = new DeleteTodoListCommand
        {
            Id = Guid.Empty,
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id is required");
    }
}