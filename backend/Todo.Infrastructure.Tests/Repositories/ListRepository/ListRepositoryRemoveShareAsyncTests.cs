namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using Dapper;
using DataAccess;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class ListRepositoryRemoveShareAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryRemoveShareAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldCallExecuteWithCorrectParameters()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var capturedCommand = new CommandDefinition();

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1) // Simulate successful deletion
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);
        Assert.Contains("DELETE FROM todo_list_shares WHERE todo_list_id = @listId AND user_id = @userId",
            capturedCommand.CommandText);

        // Verify parameters
        var listIdParam = capturedCommand.GetParameterValue<Guid>("listId");
        var userIdParam = capturedCommand.GetParameterValue<Guid>("userId");

        Assert.Equal(listId, listIdParam);
        Assert.Equal(userId, userIdParam);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldReturnTrueWhenDeleteSucceeds()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1); // One row affected (successful deletion)

        // Act
        var result = await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldReturnFalseWhenDeleteFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected (share didn't exist)

        // Act
        var result = await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldReturnFalseWhenShareDoesNotExist()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // Share doesn't exist

        // Act
        var result = await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldHandleEmptyGuids()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var validGuid = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected for invalid GUIDs

        // Test empty listId
        var resultEmptyListId = await _repository.RemoveShareAsync(emptyGuid, validGuid, CancellationToken.None);

        // Test empty userId
        var resultEmptyUserId = await _repository.RemoveShareAsync(validGuid, emptyGuid, CancellationToken.None);

        // Test both empty GUIDs
        var resultBothEmpty = await _repository.RemoveShareAsync(emptyGuid, emptyGuid, CancellationToken.None);

        // Assert all cases return false
        Assert.False(resultEmptyListId);
        Assert.False(resultEmptyUserId);
        Assert.False(resultBothEmpty);
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldThrowExceptionWhenDatabaseFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.RemoveShareAsync(listId, userId, CancellationToken.None));
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldHandleCancellation()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.RemoveShareAsync(listId, userId, cts.Token));
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldReturnFalseWhenAlreadyRemoved()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        // First call would succeed (remove existing share)
        // Second call would fail (share already removed)
        var callCount = 0;
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? 1 : 0; // First call succeeds, subsequent fail
            });

        // First attempt (success)
        var firstResult = await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Second attempt (fail - already removed)
        var secondResult = await _repository.RemoveShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.True(firstResult); // First deletion succeeds
        Assert.False(secondResult); // Second attempt fails (already removed)
    }
}
