using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.Users.GetUsers;

public class GetUsersValidatorTests
{
    private readonly GetUsersValidator _validator;

    public GetUsersValidatorTests()
    {
        _validator = new GetUsersValidator();
    }

    [Fact]
    public void GetUsersValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 0,
            Take = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetUsersValidator_NegativeSkip_FailsValidation()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = -1,
            Take = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Skip)
              .WithErrorMessage("Skip can't be less of 0");
    }

    [Fact]
    public void GetUsersValidator_ZeroTake_FailsValidation()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 0,
            Take = 0
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Take)
              .WithErrorMessage("Take can't be less of 1");
    }

    [Fact]
    public void GetUsersValidator_NegativeTake_FailsValidation()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 0,
            Take = -1
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Take)
              .WithErrorMessage("Take can't be less of 1");
    }
}