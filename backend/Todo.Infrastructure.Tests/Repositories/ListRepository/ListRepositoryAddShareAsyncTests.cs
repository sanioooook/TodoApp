namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using Dapper;
using DataAccess;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class ListRepositoryAddShareAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryAddShareAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task AddShareAsync_ShouldCallExecuteWithCorrectParameters()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var capturedCommand = new CommandDefinition();
        var capturedShareId = Guid.Empty;
        DateTime capturedCreatedAt = default;

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1) // Simulate successful insertion
            .Callback<CommandDefinition>(cmd =>
            {
                capturedCommand = cmd;
                capturedShareId = cmd.GetParameterValue<Guid>("Id");
                capturedCreatedAt = cmd.GetParameterValue<DateTime>("CreatedAt");
            });

        // Act
        await _repository.AddShareAsync(listId, userId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);
        Assert.Contains("INSERT INTO todo_list_shares", capturedCommand.CommandText);
        Assert.Contains("(id, todo_list_id, user_id, created_at)", capturedCommand.CommandText);
        Assert.Contains("VALUES (@Id, @ListId, @UserId, @CreatedAt)", capturedCommand.CommandText);

        // Verify parameters
        var listIdParam = capturedCommand.GetParameterValue<Guid>("ListId");
        var userIdParam = capturedCommand.GetParameterValue<Guid>("UserId");

        Assert.Equal(listId, listIdParam);
        Assert.Equal(userId, userIdParam);
        Assert.NotEqual(Guid.Empty, capturedShareId); // New GUID was generated
        Assert.True(capturedCreatedAt >= DateTime.UtcNow.AddMinutes(-1)); // Recent timestamp
    }

    [Fact]
    public async Task AddShareAsync_ShouldReturnTrueWhenInsertSucceeds()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1); // One row affected

        // Act
        var result = await _repository.AddShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddShareAsync_ShouldReturnFalseWhenInsertFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // No rows affected

        // Act
        var result = await _repository.AddShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddShareAsync_ShouldThrowExceptionWhenDatabaseFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.AddShareAsync(listId, userId, CancellationToken.None));
    }

    [Fact]
    public async Task AddShareAsync_ShouldHandleCancellation()
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
            _repository.AddShareAsync(listId, userId, cts.Token));
    }

    [Fact]
    public async Task AddShareAsync_ShouldHandleEmptyGuids()
    {
        // Arrange - test with empty GUIDs
        var emptyGuid = Guid.Empty;
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // Assume empty GUIDs won't insert

        // Act - test with empty listId
        var resultEmptyListId = await _repository.AddShareAsync(emptyGuid, Guid.NewGuid(), CancellationToken.None);

        // Act - test with empty userId
        var resultEmptyUserId = await _repository.AddShareAsync(Guid.NewGuid(), emptyGuid, CancellationToken.None);

        // Act - test with both empty GUIDs
        var resultBothEmpty = await _repository.AddShareAsync(emptyGuid, emptyGuid, CancellationToken.None);

        // Assert
        Assert.False(resultEmptyListId);
        Assert.False(resultEmptyUserId);
        Assert.False(resultBothEmpty);
    }

    [Fact]
    public async Task AddShareAsync_ShouldGenerateNewIdAndSetCreatedAt()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Guid capturedShareId = Guid.Empty;
        DateTime capturedCreatedAt = default;

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1)
            .Callback<CommandDefinition>(cmd =>
            {
                capturedShareId = cmd.GetParameterValue<Guid>("Id");
                capturedCreatedAt = cmd.GetParameterValue<DateTime>("CreatedAt");
            });

        // Act
        await _repository.AddShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, capturedShareId); // New GUID was generated
        Assert.True(capturedCreatedAt >= DateTime.UtcNow.AddMinutes(-1)); // Recent timestamp
    }

    [Fact]
    public async Task AddShareAsync_ShouldHandleDuplicateShares()
    {
        // Arrange - simulate duplicate share scenario
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0); // Simulate duplicate share (0 rows affected)

        // Act
        var result = await _repository.AddShareAsync(listId, userId, CancellationToken.None);

        // Assert
        Assert.False(result); // Duplicate share would return false
    }
}
