namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using Dapper;
using DataAccess;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class ListRepositoryDeleteAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryDeleteAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallExecuteWithCorrectParameters()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var capturedCommand = new CommandDefinition();

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1) // Simulate successful deletion
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.DeleteAsync(testId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);
        Assert.Contains("DELETE FROM todo_lists WHERE id = @id", capturedCommand.CommandText);

        // Verify the parameter
        var idParam = capturedCommand.GetParameterValue<Guid>("id");
        Assert.Equal(testId, idParam);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrueWhenDeleteSucceeds()
    {
        // Arrange
        var testId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1); // One row affected

        // Act
        var result = await _repository.DeleteAsync(testId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseWhenDeleteFails()
    {
        // Arrange
        var testId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected (ID not found)

        // Act
        var result = await _repository.DeleteAsync(testId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseWhenIdIsEmpty()
    {
        // Arrange
        var emptyId = Guid.Empty;
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected

        // Act
        var result = await _repository.DeleteAsync(emptyId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowExceptionWhenDatabaseFails()
    {
        // Arrange
        var testId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.DeleteAsync(testId, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ShouldHandleCancellation()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.DeleteAsync(testId, cts.Token));
    }

    [Fact]
    public async Task DeleteAsync_ShouldHandleConcurrentDeletion()
    {
        // Arrange
        var testId = Guid.NewGuid();
        // Simulate concurrent deletion where first attempt succeeds
        // but second would fail (though this is an integration scenario)
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected (already deleted)

        // Act
        var result = await _repository.DeleteAsync(testId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
