namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class ListRepositoryGetSharesAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryGetSharesAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetSharesAsync_ShouldCallQueryWithCorrectParameters()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var expectedShares = new List<TodoListShare>();
        var capturedCommand = new CommandDefinition();

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.GetSharesAsync(listId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);
        Assert.Contains("SELECT * FROM todo_list_shares WHERE todo_list_id = @listId",
            capturedCommand.CommandText);

        // Verify parameter
        var listIdParam = capturedCommand.GetParameterValue<Guid>("listId");
        Assert.Equal(listId, listIdParam);
    }

    [Fact]
    public async Task GetSharesAsync_ShouldReturnSharesForValidListId()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var expectedShares = new List<TodoListShare>
        {
            new TodoListShare { TodoListId = listId, UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new TodoListShare { TodoListId = listId, UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares);

        // Act
        var result = await _repository.GetSharesAsync(listId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(expectedShares.Count, resultList.Count);

        // Verify the returned shares match what we expect
        for (int i = 0; i < expectedShares.Count; i++)
        {
            Assert.Equal(expectedShares[i].TodoListId, resultList[i].TodoListId);
            Assert.Equal(expectedShares[i].UserId, resultList[i].UserId);
            Assert.Equal(expectedShares[i].CreatedAt, resultList[i].CreatedAt);
        }
    }

    [Fact]
    public async Task GetSharesAsync_ShouldReturnEmptyCollectionWhenNoSharesExist()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var emptyShares = Enumerable.Empty<TodoListShare>();

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(emptyShares);

        // Act
        var result = await _repository.GetSharesAsync(listId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSharesAsync_ShouldReturnEmptyCollectionForEmptyListId()
    {
        // Arrange
        var emptyListId = Guid.Empty;
        var emptyShares = Enumerable.Empty<TodoListShare>();

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(emptyShares);

        // Act
        var result = await _repository.GetSharesAsync(emptyListId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSharesAsync_ShouldThrowExceptionWhenDatabaseFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.GetSharesAsync(listId, CancellationToken.None));
    }

    [Fact]
    public async Task GetSharesAsync_ShouldHandleCancellation()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.GetSharesAsync(listId, cts.Token));
    }

    [Fact]
    public async Task GetSharesAsync_ShouldReturnProperlyFormattedShares()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var expectedShares = new List<TodoListShare>
        {
            new TodoListShare { TodoListId = listId, UserId = userId1, CreatedAt = now.AddHours(-1) },
            new TodoListShare { TodoListId = listId, UserId = userId2, CreatedAt = now }
        };

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares);

        // Act
        var result = await _repository.GetSharesAsync(listId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);

        // Verify all properties are set correctly
        Assert.Equal(listId, resultList[0].TodoListId);
        Assert.Equal(userId1, resultList[0].UserId);
        Assert.Equal(now.AddHours(-1), resultList[0].CreatedAt);

        Assert.Equal(listId, resultList[1].TodoListId);
        Assert.Equal(userId2, resultList[1].UserId);
        Assert.Equal(now, resultList[1].CreatedAt);
    }

    [Fact]
    public async Task GetSharesAsync_ShouldReturnEmptyCollectionWhenListDoesNotExist()
    {
        // Arrange
        var nonExistentListId = Guid.NewGuid();
        var emptyShares = Enumerable.Empty<TodoListShare>();

        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(emptyShares);

        // Act
        var result = await _repository.GetSharesAsync(nonExistentListId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
