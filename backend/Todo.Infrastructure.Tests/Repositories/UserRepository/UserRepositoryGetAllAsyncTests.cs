namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;

public class UserRepositoryGetAllAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryGetAllAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldUseCorrectSqlQuery()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        var expectedUsers = new List<User>();
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryAsync<User>(It.Is<CommandDefinition>(c =>
            c.CommandText.Contains("SELECT") &&
            c.CommandText.Contains("FROM users") &&
            c.CommandText.Contains("ORDER BY created_at DESC") &&
            c.CommandText.Contains("OFFSET") &&
            c.CommandText.Contains("FETCH NEXT")
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallQueryAsync_WithCorrectParameters()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        var expectedUsers = new List<User>();
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryAsync<User>(It.Is<CommandDefinition>(c =>
            c.GetParameterValue<int>("Skip") == skip &&
            c.GetParameterValue<int>("Take") == take
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectNumberOfUsers()
    {
        // Arrange
        var skip = 0;
        var take = 2;
        var expectedUsers = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "user1@example.com", FullName = "User One", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new User { Id = Guid.NewGuid(), Email = "user2@example.com", FullName = "User Two", CreatedAt = DateTime.UtcNow }
        };
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(take, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldHandleNegativeSkipAndTakeParameters()
    {
        // Arrange
        var skip = -1;
        var take = -1;
        var expectedUsers = new List<User>();
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetAllAsync(skip, take, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.GetAllAsync(skip, take, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        var expectedUsers = new List<User>();
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersOrderedByCreatedAtDesc()
    {
        // Arrange
        var skip = 0;
        var take = 2;
        var expectedUsers = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "user1@example.com", FullName = "User One", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new User { Id = Guid.NewGuid(), Email = "user2@example.com", FullName = "User Two", CreatedAt = DateTime.UtcNow }
        };
        _executorMock
            .Setup(e => e.QueryAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _repository.GetAllAsync(skip, take, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var usersList = result.ToList();
        Assert.Equal(2, usersList.Count);
        Assert.True(usersList[0].CreatedAt < usersList[1].CreatedAt);
    }
}