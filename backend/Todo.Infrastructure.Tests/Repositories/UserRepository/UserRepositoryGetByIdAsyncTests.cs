using Todo.Infrastructure.Tests.Helpers;

namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Moq;
using Todo.Infrastructure.Repositories;

public class UserRepositoryGetByIdAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryGetByIdAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User { Id = userId, Email = "test@example.com", FullName = "Test User" };

        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.Is<CommandDefinition>(c =>
                c.CommandText.Contains("FROM users") &&
                c.GetParameterValue<Guid>("Id") == userId)))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /*[Fact]
    public async Task GetByIdAsync_ShouldCallQueryFirstOrDefaultAsync_WithCorrectParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User { Id = userId, Email = "test@example.com", FullName = "Test User" };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUser);

        // Act
        await _repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryFirstOrDefaultAsync<User>(It.Is<CommandDefinition>(c =>
            c.CommandText.Contains("SELECT * FROM users WHERE id = @Id") &&
            c.GetParameterValue<Guid>("Id") == userId
        )), Times.Once);
    }*/

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdIsEmpty()
    {
        // Arrange
        var emptyId = Guid.Empty;
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _repository.GetByIdAsync(emptyId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetByIdAsync(userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.GetByIdAsync(userId, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserWithCorrectProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User
        {
            Id = userId,
            Email = "correct@example.com",
            FullName = "Correct User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.FullName, result.FullName);
        Assert.Equal(expectedUser.CreatedAt, result.CreatedAt);
    }
}