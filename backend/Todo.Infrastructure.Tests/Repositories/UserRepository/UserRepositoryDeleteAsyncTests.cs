namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using System.Threading;
using Dapper;
using DataAccess;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;
using Xunit;

public class UserRepositoryDeleteAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryDeleteAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.DeleteAsync(id, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Contains("DELETE FROM users WHERE id = @Id", capturedCommand.CommandText);

        var idParam = capturedCommand.GetParameterValue<Guid>("Id");
        Assert.Equal(id, idParam);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDeleteIsSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1); 

        // Act
        var result = await _repository.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenDeleteIsUnsuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenIdIsEmpty()
    {
        // Arrange
        var emptyId = Guid.Empty;
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.DeleteAsync(emptyId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.DeleteAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.DeleteAsync(id, cancellationTokenSource.Token));
    }
}