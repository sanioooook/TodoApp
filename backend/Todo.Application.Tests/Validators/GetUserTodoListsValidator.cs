using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.GetUserTodoLists;

public class GetUserTodoListsValidatorTests
{
    private readonly GetUserTodoListsValidator _validator;

    public GetUserTodoListsValidatorTests()
    {
        _validator = new GetUserTodoListsValidator();
    }

    [Fact]
    public void GetUserTodoListsValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetUserTodoListsQuery
        {
            UserId = Guid.NewGuid(),
            Skip = 0,
            Take = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetUserTodoListsValidator_EmptyUserId_FailsValidation()
    {
        // Arrange
        var query = new GetUserTodoListsQuery
        {
            UserId = Guid.Empty,
            Skip = 0,
            Take = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
              .WithErrorMessage("UserId is required. Maybe X-User-Id header is missing or invalid");
    }

    [Fact]
    public void GetUserTodoListsValidator_NegativeSkip_FailsValidation()
    {
        // Arrange
        var query = new GetUserTodoListsQuery
        {
            UserId = Guid.NewGuid(),
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
    public void GetUserTodoListsValidator_ZeroTake_FailsValidation()
    {
        // Arrange
        var query = new GetUserTodoListsQuery
        {
            UserId = Guid.NewGuid(),
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
    public void GetUserTodoListsValidator_NegativeTake_FailsValidation()
    {
        // Arrange
        var query = new GetUserTodoListsQuery
        {
            UserId = Guid.NewGuid(),
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