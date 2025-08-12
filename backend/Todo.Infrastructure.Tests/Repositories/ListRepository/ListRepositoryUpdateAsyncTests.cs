namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class ListRepositoryUpdateAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryUpdateAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetUpdatedAtBeforeExecution()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1) // Old timestamp
        };

        var beforeCall = DateTime.UtcNow;
        DateTime capturedUpdatedAt = default;

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd =>
            {
                capturedUpdatedAt = cmd.GetParameterValue<DateTime>("UpdatedAt");
            });

        // Act
        await _repository.UpdateAsync(list, CancellationToken.None);

        // Assert
        // Check that UpdatedAt was modified in the list object
        Assert.True(list.UpdatedAt >= beforeCall);

        // Check that the parameter passed to SQL has the same UpdatedAt
        Assert.Equal(list.UpdatedAt, capturedUpdatedAt);

        // Check that we're within reasonable time range
        Assert.True(Math.Abs((DateTime.UtcNow - list.UpdatedAt).TotalSeconds) < 5);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallExecuteAsyncWithCorrectParameters()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Test List",
            OwnerId = Guid.NewGuid()
        };

        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.UpdateAsync(list, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);

        // Verify SQL structure
        Assert.Contains("UPDATE todo_lists", capturedCommand.CommandText);
        Assert.Contains("SET title = @Title, updated_at = @UpdatedAt", capturedCommand.CommandText);
        Assert.Contains("WHERE id = @Id", capturedCommand.CommandText);

        // Verify parameters
        var idParam = capturedCommand.GetParameterValue<Guid>("Id");
        var titleParam = capturedCommand.GetParameterValue<string>("Title");
        var updatedAtParam = capturedCommand.GetParameterValue<DateTime>("UpdatedAt");

        Assert.Equal(list.Id, idParam);
        Assert.Equal(list.Title, titleParam);
        Assert.True(updatedAtParam >= DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrueWhenUpdateSucceeds()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Successful Update",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1); // One row affected

        // Act
        var result = await _repository.UpdateAsync(list, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalseWhenUpdateFails()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Failed Update",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected

        // Act
        var result = await _repository.UpdateAsync(list, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullExceptionWhenListIsNull()
    {
        // Arrange - pass null as the list parameter

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.UpdateAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowExceptionWhenDatabaseOperationFails()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Problem List",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.UpdateAsync(list, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ShouldHandleCancellation()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Cancelled Update",
            OwnerId = Guid.NewGuid()
        };
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.UpdateAsync(list, cts.Token));
    }

    [Fact]
    public async Task UpdateAsync_ShouldHandleEmptyTitle()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "", // Empty title
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.UpdateAsync(list, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}