using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.Users.UpdateUser;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator;

    public UpdateUserValidatorTests()
    {
        _validator = new UpdateUserValidator();
    }

    [Fact]
    public void UpdateUserValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateUserValidator_EmptyId_FailsValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.Empty,
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id is required");
    }

    [Fact]
    public void UpdateUserValidator_EmptyEmail_FailsValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
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
    public void UpdateUserValidator_InvalidEmail_FailsValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
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
    public void UpdateUserValidator_EmptyFullName_FailsValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("FullName is required");
    }

    [Fact]
    public void UpdateUserValidator_NullEmail_PassesValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Email = null,
            FullName = "Test User"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required");
    }

    [Fact]
    public void UpdateUserValidator_NullFullName_FailsValidation()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("FullName is required");
    }
}