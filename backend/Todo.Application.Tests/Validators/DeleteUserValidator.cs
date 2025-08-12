using FluentValidation.TestHelper;
using Todo.Application.UseCases.Users.DeleteUser;

namespace Todo.Application.Tests.Validators;

public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUserValidator();
    }

    [Fact]
    public void DeleteUserValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeleteUserValidator_EmptyId_FailsValidation()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }
}