using Todo.Infrastructure.Tests.Helpers;

namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Moq;
using Todo.Infrastructure.Repositories;

public class UserRepositoryGetByEmailAsyncTest
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryGetByEmailAsyncTest()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailMatches()
    {
        // Arrange
        var email = "john@example.com";
        var expectedUser = new User { Id = Guid.NewGuid(), Email = email, FullName = "John Doe" };

        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.Is<CommandDefinition>(c =>
                c.GetParameterValue<string>("email") == email)))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Email, result.Email);
    }

    /*[Fact]
    public async Task GetByEmailAsync_ShouldCallQueryFirstOrDefaultAsync_WithCorrectParameters()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new User { Id = Guid.NewGuid(), Email = email, FullName = "Test User" };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUser);

        // Act
        await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.QueryFirstOrDefaultAsync<User>(It.Is<CommandDefinition>(c =>
            c.CommandText.Contains("SELECT * FROM users WHERE email = @email") &&
            c.GetParameterValue<string>("email") == email
        )), Times.Once);
    }*/

    [Fact]
    public async Task GetByEmailAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var email = "test@example.com";
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetByEmailAsync(email, CancellationToken.None));
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var email = "test@example.com";
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.GetByEmailAsync(email, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserWithCorrectProperties()
    {
        // Arrange
        var email = "correct@example.com";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Correct User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.QueryFirstOrDefaultAsync<User>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.FullName, result.FullName);
        Assert.Equal(expectedUser.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldThrowArgumentException_WhenEmailIsNullOrEmpty()
    {
        // Arrange
        string nullEmail = null;
        var emptyEmail = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByEmailAsync(nullEmail, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByEmailAsync(emptyEmail, CancellationToken.None));
    }
}