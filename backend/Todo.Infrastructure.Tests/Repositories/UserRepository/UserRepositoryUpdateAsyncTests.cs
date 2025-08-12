namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using System.Threading;
using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;
using Xunit;

public class UserRepositoryUpdateAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryUpdateAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "updated@example.com",
            FullName = "Updated User",
            CreatedAt = DateTime.UtcNow
        };
        CommandDefinition capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Contains("UPDATE users", capturedCommand.CommandText);
        Assert.Contains("WHERE id = @Id", capturedCommand.CommandText);

        var idParam = capturedCommand.GetParameterValue<Guid>("Id");
        var emailParam = capturedCommand.GetParameterValue<string>("Email");
        var fullNameParam = capturedCommand.GetParameterValue<string>("FullName");

        Assert.Equal(user.Id, idParam);
        Assert.Equal(user.Email, emailParam);
        Assert.Equal(user.FullName, fullNameParam);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "success@example.com",
            FullName = "Successful Update",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenUpdateIsUnsuccessful()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "fail@example.com",
            FullName = "Failed Update",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenUserIdIsEmpty()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.Empty,
            Email = "emptyid@example.com",
            FullName = "Empty ID User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "exception@example.com",
            FullName = "Exception User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.UpdateAsync(user, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "cancel@example.com",
            FullName = "Cancel User",
            CreatedAt = DateTime.UtcNow
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.UpdateAsync(user, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        User user = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(user, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAllFields()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "updateall@example.com",
            FullName = "Update All User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.Is<CommandDefinition>(c =>
            c.GetParameterValue<string>("Email") == user.Email &&
            c.GetParameterValue<string>("FullName") == user.FullName
        )), Times.Once);
    }
}