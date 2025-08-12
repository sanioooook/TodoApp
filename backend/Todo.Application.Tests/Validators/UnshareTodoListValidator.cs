using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.UnshareTodoList;

public class UnshareTodoListValidatorTests
{
    private readonly UnshareTodoListValidator _validator;

    public UnshareTodoListValidatorTests()
    {
        _validator = new UnshareTodoListValidator();
    }

    [Fact]
    public void UnshareTodoListValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UnshareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            CurrentUserId = Guid.NewGuid(),
            TargetUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UnshareTodoListValidator_EmptyCurrentUserId_FailsValidation()
    {
        // Arrange
        var command = new UnshareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            CurrentUserId = Guid.Empty,
            TargetUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentUserId)
              .WithErrorMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");
    }

    [Fact]
    public void UnshareTodoListValidator_EmptyTargetUserId_FailsValidation()
    {
        // Arrange
        var command = new UnshareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            CurrentUserId = Guid.NewGuid(),
            TargetUserId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TargetUserId)
              .WithErrorMessage("TargetUserId is required");
    }

    [Fact]
    public void UnshareTodoListValidator_EmptyListId_FailsValidation()
    {
        // Arrange
        var command = new UnshareTodoListCommand
        {
            ListId = Guid.Empty,
            CurrentUserId = Guid.NewGuid(),
            TargetUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ListId)
              .WithErrorMessage("ListId is required");
    }
}