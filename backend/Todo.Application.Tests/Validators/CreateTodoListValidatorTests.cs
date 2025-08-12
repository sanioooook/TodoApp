using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.CreateTodoList;

public class CreateTodoListValidatorTests
{
    private readonly CreateTodoListValidator _validator;

    public CreateTodoListValidatorTests()
    {
        _validator = new CreateTodoListValidator();
    }

    [Fact]
    public void CreateTodoListValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "Valid Title",
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateTodoListValidator_EmptyTitle_FailsValidation()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "",
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Minimum length Title is 1 symbol");
    }

    [Fact]
    public void CreateTodoListValidator_LongTitle_FailsValidation()
    {
        // Arrange
        var longTitle = new string('a', 256);
        var command = new CreateTodoListCommand
        {
            Title = longTitle,
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Maximum length Title is 255 symbol");
    }

    [Fact]
    public void CreateTodoListValidator_EmptyOwnerId_FailsValidation()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "Valid Title",
            OwnerId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OwnerId)
            .WithErrorMessage("OwnerId is required");
    }
}