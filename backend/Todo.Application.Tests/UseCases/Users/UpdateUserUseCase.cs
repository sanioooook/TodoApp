namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.UpdateUser;

public class UpdateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _useCase = new UpdateUserUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task UpdateUserUseCaseTests_UpdateExistingUser_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newEmail = "new@example.com";
        var newFullName = "New Name";

        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = newEmail,
            FullName = newFullName
        };

        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            FullName = "Old Name",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        _mockRepository.Setup(x => x.GetByEmailAsync(newEmail, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully updated User", result.Message);
    }

    [Fact]
    public async Task UpdateUserUseCaseTests_UpdateNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newEmail = "new@example.com";
        var newFullName = "New Name";

        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = newEmail,
            FullName = newFullName
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("User not found", result.Message);
    }

    [Fact]
    public async Task UpdateUserUseCaseTests_UpdateWithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingEmail = "existing@example.com";
        var newFullName = "New Name";

        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = existingEmail,
            FullName = newFullName
        };

        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            FullName = "Old Name",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var conflictingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = existingEmail,
            FullName = "Conflicting User",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        _mockRepository.Setup(x => x.GetByEmailAsync(existingEmail, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(conflictingUser);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.Conflict, result.CodeResult);
        Assert.Contains("Email is already in use", result.Message);
    }

    [Fact]
    public async Task UpdateUserUseCaseTests_UpdateSameEmail_DoesNotCheckConflict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingEmail = "existing@example.com";
        var newFullName = "New Name";

        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = existingEmail,
            FullName = newFullName
        };

        var existingUser = new User
        {
            Id = userId,
            Email = existingEmail,
            FullName = "Old Name",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully updated User", result.Message);
    }

    [Fact]
    public async Task UpdateUserUseCaseTests_FailedToUpdate_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newEmail = "new@example.com";
        var newFullName = "New Name";

        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = newEmail,
            FullName = newFullName
        };

        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            FullName = "Old Name",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        _mockRepository.Setup(x => x.GetByEmailAsync(newEmail, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while updating User", result.Message);
    }
}