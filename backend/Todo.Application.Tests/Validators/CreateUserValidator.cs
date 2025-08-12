using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.Users.CreateUser;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator();
    }

    [Fact]
    public void CreateUserValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateUserValidator_EmptyEmail_FailsValidation()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "",
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required");
    }

    [Fact]
    public void CreateUserValidator_InvalidEmail_FailsValidation()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "invalid-email",
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email format");
    }

    [Fact]
    public void CreateUserValidator_EmptyFullName_FailsValidation()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("FullName is required");
    }
}