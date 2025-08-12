using FluentValidation.TestHelper;
using Todo.Application.UseCases.Users.GetUserById;

namespace Todo.Application.Tests.Validators;

public class GetUserByIdValidatorTests
{
    private readonly GetUserByIdValidator _validator;

    public GetUserByIdValidatorTests()
    {
        _validator = new GetUserByIdValidator();
    }

    [Fact]
    public void GetUserByIdValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetUserByIdQuery
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetUserByIdValidator_EmptyId_FailsValidation()
    {
        // Arrange
        var query = new GetUserByIdQuery
        {
            Id = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }
}