using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.UpdateTodoList;

public class UpdateTodoListValidatorTests
{
    private readonly UpdateTodoListValidator _validator;

    public UpdateTodoListValidatorTests()
    {
        _validator = new UpdateTodoListValidator();
    }

    [Fact]
    public void UpdateTodoListValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UpdateTodoListCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateTodoListValidator_EmptyCurrentUserId_FailsValidation()
    {
        // Arrange
        var command = new UpdateTodoListCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            CurrentUserId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentUserId)
              .WithErrorMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");
    }

    [Fact]
    public void UpdateTodoListValidator_EmptyTitle_FailsValidation()
    {
        // Arrange
        var command = new UpdateTodoListCommand
        {
            Id = Guid.NewGuid(),
            Title = "",
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title is required");
    }

    [Fact]
    public void UpdateTodoListValidator_ShortTitle_FailsValidation()
    {
        // Arrange
        var command = new UpdateTodoListCommand
        {
            Id = Guid.NewGuid(),
            Title = "",
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Minimum length Title is 1 symbol");
    }

    [Fact]
    public void UpdateTodoListValidator_LongTitle_FailsValidation()
    {
        // Arrange
        var longTitle = new string('a', 256);
        var command = new UpdateTodoListCommand
        {
            Id = Guid.NewGuid(),
            Title = longTitle,
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Maximum length Title is 255 symbol");
    }

    [Fact]
    public void UpdateTodoListValidator_EmptyId_FailsValidation()
    {
        // Arrange
        var command = new UpdateTodoListCommand
        {
            Id = Guid.Empty,
            Title = "Valid Title",
            CurrentUserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id is required");
    }
}