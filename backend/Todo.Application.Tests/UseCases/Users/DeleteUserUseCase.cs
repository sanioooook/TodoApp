namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.DeleteUser;

public class DeleteUserUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserUseCaseTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _useCase = new DeleteUserUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task DeleteUserUseCaseTests_DeleteExistingUser_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new DeleteUserCommand
        {
            Id = userId
        };

        var mockUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockUser);

        _mockRepository.Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("User successfully deleted", result.Message);
    }

    [Fact]
    public async Task DeleteUserUseCaseTests_DeleteNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new DeleteUserCommand
        {
            Id = userId
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
    public async Task DeleteUserUseCaseTests_FailedToDeleteUser_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new DeleteUserCommand
        {
            Id = userId
        };

        var mockUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockUser);

        _mockRepository.Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while deleting User", result.Message);
    }
}