using Todo.Application.Enums;

namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.DeleteTodoList;

public class DeleteTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly DeleteTodoListUseCase _useCase;

    public DeleteTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _useCase = new DeleteTodoListUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task DeleteTodoListUseCaseTests_DeleteWithValidOwner_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeleteTodoListCommand
        {
            Id = listId,
            CurrentUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTodoList);
        _mockRepository.Setup(x => x.DeleteAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully deleted", result.Message);
    }

    [Fact]
    public async Task DeleteTodoListUseCaseTests_DeleteNonExistingList_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteTodoListCommand
        {
            Id = listId,
            CurrentUserId = userId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList)null);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("TodoList not found", result.Message);
    }

    [Fact]
    public async Task DeleteTodoListUseCaseTests_DeleteByNonOwner_ReturnsForbidden()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var command = new DeleteTodoListCommand
        {
            Id = listId,
            CurrentUserId = nonOwnerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.Forbidden, result.CodeResult);
        Assert.Contains("TodoList can't be deleted", result.Message);
    }

    [Fact]
    public async Task DeleteTodoListUseCaseTests_FailedToDelete_ReturnsError()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeleteTodoListCommand
        {
            Id = listId,
            CurrentUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTodoList);
        _mockRepository.Setup(x => x.DeleteAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while deleting", result.Message);
    }
}