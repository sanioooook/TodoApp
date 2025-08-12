namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;
using Xunit;

public class ListRepositoryGetForUserAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryGetForUserAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetForUserAsync_ShouldCallQueryAsync_WithCorrectParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 10;
        var expectedLists = new List<TodoList>
        {
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 1" },
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 2" }
        };
        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedLists)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.GetForUserAsync(userId, skip, take, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);

        // Проверяем ключевые части SQL-запроса
        Assert.Contains("SELECT DISTINCT tl.* FROM todo_lists tl", capturedCommand.CommandText);
        Assert.Contains("LEFT JOIN todo_list_shares tls ON tls.todo_list_id = tl.id", capturedCommand.CommandText);
        Assert.Contains("WHERE tl.owner_id = @userId OR tls.user_id = @userId", capturedCommand.CommandText);
        Assert.Contains("ORDER BY tl.created_at DESC", capturedCommand.CommandText);
        Assert.Contains("OFFSET @skip LIMIT @take", capturedCommand.CommandText);

        // Проверяем параметры
        var userIdParam = capturedCommand.GetParameterValue<Guid>("userId");
        var skipParam = capturedCommand.GetParameterValue<int>("skip");
        var takeParam = capturedCommand.GetParameterValue<int>("take");
        Assert.Equal(userId, userIdParam);
        Assert.Equal(skip, skipParam);
        Assert.Equal(take, takeParam);
    }

    [Fact]
    public async Task GetForUserAsync_ShouldReturnCorrectNumberOfLists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 2;
        var expectedLists = new List<TodoList>
        {
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 1", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 2", CreatedAt = DateTime.UtcNow }
        };
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedLists);

        // Act
        var result = await _repository.GetForUserAsync(userId, skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(take, result.Count());
        Assert.Equal(expectedLists.First().Id, result.First().Id);
        Assert.Equal(expectedLists.Last().Id, result.Last().Id);
    }

    [Fact]
    public async Task GetForUserAsync_ShouldHandleNegativeSkipAndTakeParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = -1;
        var take = -1;
        var expectedLists = new List<TodoList>();
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedLists);

        // Act
        var result = await _repository.GetForUserAsync(userId, skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForUserAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 10;
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetForUserAsync(userId, skip, take, CancellationToken.None));
    }

    [Fact]
    public async Task GetForUserAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 10;
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.GetForUserAsync(userId, skip, take, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetForUserAsync_ShouldReturnEmptyList_WhenNoListsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 10;
        var expectedLists = new List<TodoList>();
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedLists);

        // Act
        var result = await _repository.GetForUserAsync(userId, skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForUserAsync_ShouldReturnListsOrderedByCreatedAtDesc()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skip = 0;
        var take = 2;
        var expectedLists = new List<TodoList>
        {
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 1", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new TodoList { Id = Guid.NewGuid(), OwnerId = userId, Title = "List 2", CreatedAt = DateTime.UtcNow }
        };
        _executorMock
            .Setup(e => e.QueryAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedLists.OrderByDescending(x => x.CreatedAt));

        // Act
        var result = await _repository.GetForUserAsync(userId, skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var listsArray = result.ToArray();
        Assert.Equal(2, listsArray.Length);
        Assert.True(listsArray[0].CreatedAt > listsArray[1].CreatedAt);
    }
}