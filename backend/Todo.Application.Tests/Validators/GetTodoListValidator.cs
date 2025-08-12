using FluentValidation.TestHelper;
using Todo.Application.UseCases.TodoLists.GetTodoList;

namespace Todo.Application.Tests.Validators;

public class GetTodoListValidatorTests
{
    private readonly GetTodoListValidator _validator;

    public GetTodoListValidatorTests()
    {
        _validator = new GetTodoListValidator();
    }

    [Fact]
    public void GetTodoListValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetTodoListQuery
        {
            UserId = Guid.NewGuid(),
            ListId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetTodoListValidator_EmptyUserId_FailsValidation()
    {
        // Arrange
        var query = new GetTodoListQuery
        {
            UserId = Guid.Empty,
            ListId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required. Maybe X-User-Id header is missing or invalid");
    }

    [Fact]
    public void GetTodoListValidator_EmptyListId_FailsValidation()
    {
        // Arrange
        var query = new GetTodoListQuery
        {
            UserId = Guid.NewGuid(),
            ListId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ListId)
            .WithErrorMessage("ListId is required");
    }
}