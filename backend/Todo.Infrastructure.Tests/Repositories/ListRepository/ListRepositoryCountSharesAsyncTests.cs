namespace Todo.Infrastructure.Tests.Repositories.ListRepository;

using System.Threading;
using Dapper;
using DataAccess;
using Domain.Entities;
using Helpers;
using Moq;
using Todo.Infrastructure.Repositories;
using Xunit;

public class ListRepositoryCountSharesAsyncTests
{
    private readonly Mock<IDapperExecutor> _executorMock;
    private readonly ListRepository _repository;

    public ListRepositoryCountSharesAsyncTests()
    {
        _executorMock = new Mock<IDapperExecutor>();
        _repository = new ListRepository(_executorMock.Object);
    }

    [Fact]
    public async Task CountSharesAsync_ShouldCallExecuteScalarAsync_WithCorrectParameters()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var expectedCount = 3; // Предположим, что у списка 3 совместных пользователя
        var capturedCommand = new CommandDefinition();
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedCount)
            .Callback<CommandDefinition>(cmd => capturedCommand = cmd);

        // Act
        await _repository.CountSharesAsync(listId, CancellationToken.None);

        // Assert
        _executorMock.Verify(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()), Times.Once);
        Assert.NotNull(capturedCommand);
        Assert.Contains("SELECT COUNT(*) FROM todo_list_shares WHERE todo_list_id = @listId", capturedCommand.CommandText);

        // Проверяем параметры
        var listIdParam = capturedCommand.GetParameterValue<Guid>("listId");
        Assert.Equal(listId, listIdParam);
    }

    [Fact]
    public async Task CountSharesAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var expectedCount = 3;
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _repository.CountSharesAsync(listId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedCount, result);
    }

    [Fact]
    public async Task CountSharesAsync_ShouldReturnZero_WhenListIdIsEmpty()
    {
        // Arrange
        var emptyListId = Guid.Empty;
        var expectedCount = 0; // Ожидаем 0 для пустого ID
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _repository.CountSharesAsync(emptyListId, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountSharesAsync_ShouldThrowException_WhenDatabaseOperationFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.CountSharesAsync(listId, CancellationToken.None));
    }

    [Fact]
    public async Task CountSharesAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _repository.CountSharesAsync(listId, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task CountSharesAsync_ShouldReturnZero_WhenNoSharesExist()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var expectedCount = 0; // Нет совместных доступов
        _executorMock
            .Setup(e => e.ExecuteScalarAsync<int>(It.IsAny<CommandDefinition>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _repository.CountSharesAsync(listId, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }
}