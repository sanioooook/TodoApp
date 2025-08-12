using Todo.Infrastructure.Tests.Helpers;

namespace Todo.Infrastructure.Tests.Repositories.UserRepository;

using Dapper;
using DataAccess;
using Domain.Entities;
using Moq;
using Todo.Infrastructure.Repositories;

public class UserRepositoryAddAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly UserRepository _repository;

    public UserRepositoryAddAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new UserRepository(_executorMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "new@example.com",
            FullName = "New User",
            CreatedAt = DateTime.UtcNow
        };

        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        await _repository.AddAsync(user, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.Is<CommandDefinition>(c =>
            c.GetParameterValue<string>("Email") == user.Email &&
            c.GetParameterValue<string>("FullName") == user.FullName
        )), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenInsertIsSuccessful()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "success@example.com",
            FullName = "Successful User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.AddAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFalse_WhenInsertIsUnsuccessful()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "fail@example.com",
            FullName = "Unsuccessful User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.AddAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldPassAllParametersCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "params@example.com",
            FullName = "Parameterized User",
            CreatedAt = DateTime.UtcNow
        };
        _executorMock
            .Setup(e => e.ExecuteAsync(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(1);

        // Act
        await _repository.AddAsync(user, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteAsync(It.Is<CommandDefinition>(c =>
            c.GetParameterValue<Guid>("Id") == user.Id &&
            c.GetParameterValue<string>("Email") == user.Email &&
            c.GetParameterValue<string>("FullName") == user.FullName &&
            c.GetParameterValue<DateTime>("CreatedAt") == user.CreatedAt
        )), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        User user = null;

        // Act & Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(user, CancellationToken.None));
    }
}