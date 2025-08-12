using FluentValidation.TestHelper;

namespace Todo.Application.Tests.Validators;

using Todo.Application.UseCases.TodoLists.ShareTodoList;

public class ShareTodoListValidatorTests
{
    private readonly ShareTodoListValidator _validator;

    public ShareTodoListValidatorTests()
    {
        _validator = new ShareTodoListValidator();
    }

    [Fact]
    public void ShareTodoListValidator_ValidCommand_PassesValidation()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShareTodoListValidator_EmptyCurrentUserId_FailsValidation()
    {
        // Arrange
        var command = new ShareTodoListCommand
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
    public void ShareTodoListValidator_EmptyTargetUserId_FailsValidation()
    {
        // Arrange
        var command = new ShareTodoListCommand
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
    public void ShareTodoListValidator_TargetUserIdEqualCurrentUserId_FailsValidation()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            CurrentUserId = userId,
            TargetUserId = userId
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TargetUserId)
              .WithErrorMessage("TargetUserId must not equal CurrentUserId");
    }

    [Fact]
    public void ShareTodoListValidator_EmptyListId_FailsValidation()
    {
        // Arrange
        var command = new ShareTodoListCommand
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