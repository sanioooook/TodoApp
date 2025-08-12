namespace Todo.Infrastructure.Tests.Repositories.ListRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper;
using DataAccess;
using Domain.Entities;
using Moq;
using Todo.Infrastructure.Repositories;
using Todo.Infrastructure.Tests.Helpers;
using Xunit;

public class ListRepositoryGetByIdAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryGetByIdAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCallQueryFirstOrDefaultAsync_WithCorrectParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedList = new TodoList { Id = id, OwnerId = userId, Title = "Test List" };
        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedList)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.GetByIdAsync(id, userId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()), Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Contains("SELECT tl.* FROM todo_lists tl", capturedCommand.CommandText);
        Assert.Contains("WHERE tl.id = @id", capturedCommand.CommandText);
        Assert.Contains("tl.owner_id = @userId", capturedCommand.CommandText);

        var idParam = capturedCommand.GetParameterValue<Guid>("id");
        var userIdParam = capturedCommand.GetParameterValue<Guid>("userId");
        Assert.Equal(id, idParam);
        Assert.Equal(userId, userIdParam);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoList_WhenListExistsAndUserHasAccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedList = new TodoList { Id = id, OwnerId = userId, Title = "Test List" };
        var expectedShares = new List<TodoListShare>
        {
            new TodoListShare { TodoListId = id, UserId = Guid.NewGuid() },
            new TodoListShare { TodoListId = id, UserId = Guid.NewGuid() }
        };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedList);
        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares);

        // Act
        var result = await _repository.GetByIdAsync(id, userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedList.Id, result.Id);
        Assert.Equal(expectedList.Title, result.Title);
        Assert.Equal(expectedList.OwnerId, result.OwnerId);
        Assert.Equal(2, result.Shares.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenListDoesNotExistOrUserHasNoAccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync((TodoList)null);

        // Act
        var result = await _repository.GetByIdAsync(id, userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetByIdAsync(id, userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.GetByIdAsync(id, userId, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLoadShares_WhenListExistsAndUserHasAccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedList = new TodoList { Id = id, OwnerId = userId, Title = "Test List" };
        var expectedShares = new List<TodoListShare>
        {
            new TodoListShare { TodoListId = id, UserId = Guid.NewGuid() },
            new TodoListShare { TodoListId = id, UserId = Guid.NewGuid() }
        };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedList);
        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares);

        // Act
        var result = await _repository.GetByIdAsync(id, userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Shares.Count());
        Assert.Equal(expectedShares.First().UserId, result.Shares.First().UserId);
        Assert.Equal(expectedShares.Last().UserId, result.Shares.Last().UserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoList_WhenUserIsOwner()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedList = new TodoList { Id = id, OwnerId = userId, Title = "Test List" };
        var expectedShares = new List<TodoListShare>();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<TodoList>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedList);
        _executorMock
            .Setup(e => e.QueryAsync<TodoListShare>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedShares);

        // Act
        var result = await _repository.GetByIdAsync(id, userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedList.Id, result.Id);
        Assert.Equal(expectedList.Title, result.Title);
        Assert.Equal(expectedList.OwnerId, result.OwnerId);
        Assert.Empty(result.Shares);
    }
}