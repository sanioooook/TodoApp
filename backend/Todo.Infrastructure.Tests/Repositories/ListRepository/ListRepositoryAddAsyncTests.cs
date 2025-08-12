namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using System.Threading;
using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;
using Xunit;

public class ListRepositoryAddAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryAddAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "New List",
            OwnerId = Guid.NewGuid()
        };
        var expectedAffectedRows = 1;

        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedAffectedRows)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.AddAsync(list, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Contains("INSERT INTO todo_lists (id, title, owner_id, created_at, updated_at)", capturedCommand.CommandText);
        Assert.Contains("VALUES (@Id, @Title, @OwnerId, @CreatedAt, @UpdatedAt)", capturedCommand.CommandText);

        // Проверяем параметры
        var idParam = capturedCommand.GetParameterValue<Guid>("Id");
        var titleParam = capturedCommand.GetParameterValue<string>("Title");
        var ownerIdParam = capturedCommand.GetParameterValue<Guid>("OwnerId");
        var createdAtParam = capturedCommand.GetParameterValue<DateTime>("CreatedAt");
        var updatedAtParam = capturedCommand.GetParameterValue<DateTime>("UpdatedAt");

        Assert.Equal(list.Id, idParam);
        Assert.Equal(list.Title, titleParam);
        Assert.Equal(list.OwnerId, ownerIdParam);
        Assert.True(createdAtParam > DateTime.UtcNow.AddMinutes(-1));
        Assert.True(updatedAtParam > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenInsertIsSuccessful()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Successful List",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.AddAsync(list, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFalse_WhenInsertIsUnsuccessful()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Failed List",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); 

        // Act
        var result = await _repository.AddAsync(list, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAtAndUpdatedAt()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Timestamp Test List",
            OwnerId = Guid.NewGuid()
        };

        var beforeCall = DateTime.UtcNow;
        DateTime capturedCreatedAt = default;
        DateTime capturedUpdatedAt = default;

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd =>
            {
                capturedCreatedAt = cmd.GetParameterValue<DateTime>("CreatedAt");
                capturedUpdatedAt = cmd.GetParameterValue<DateTime>("UpdatedAt");
            });

        // Act
        await _repository.AddAsync(list, CancellationToken.None);

        // Assert
        Assert.True(list.CreatedAt >= beforeCall);
        Assert.True(list.UpdatedAt >= beforeCall);

        Assert.Equal(list.CreatedAt, capturedCreatedAt);
        Assert.Equal(list.UpdatedAt, capturedUpdatedAt);

        Assert.True(capturedCreatedAt >= beforeCall);
        Assert.True(capturedUpdatedAt >= beforeCall);
        Assert.True(Math.Abs((capturedCreatedAt - capturedUpdatedAt).TotalMilliseconds) < 100);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Exception List",
            OwnerId = Guid.NewGuid()
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.AddAsync(list, CancellationToken.None));
    }

    [Fact]
    public async Task AddAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Cancelled List",
            OwnerId = Guid.NewGuid()
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.AddAsync(list, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task AddAsync_ShouldHandleInvalidData()
    {
        // Arrange
        var listWithEmptyId = new TodoList
        {
            Id = Guid.Empty,
            Title = "Empty ID List",
            OwnerId = Guid.NewGuid()
        };

        var listWithNoOwnerId = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "No Owner List"
        };


        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var resultWithEmptyId = await _repository.AddAsync(listWithEmptyId, CancellationToken.None);
        var resultWithNoOwnerId = await _repository.AddAsync(listWithNoOwnerId, CancellationToken.None);

        // Assert
        Assert.False(resultWithEmptyId);
        Assert.False(resultWithNoOwnerId);
    }
}