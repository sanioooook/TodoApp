namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.UpdateTodoList;

public class UpdateTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly UpdateTodoListUseCase _useCase;

    public UpdateTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _useCase = new UpdateTodoListUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_UpdateByOwner_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var newTitle = "Updated Title";

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Old Title",
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully update TodoList", result.Message);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_UpdateBySharedUser_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var newTitle = "Updated Title";

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = sharedUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Old Title",
            OwnerId = ownerId,
            Shares = new List<TodoListShare>
                {
                    new TodoListShare { UserId = sharedUserId }
                }
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, sharedUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully update TodoList", result.Message);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_UpdateNonExistingList_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var newTitle = "Updated Title";

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = userId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((TodoList)null);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("TodoList not found", result.Message);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_UpdateByNonOwnerAndNotSharedUser_ReturnsForbidden()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var newTitle = "Updated Title";

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = nonOwnerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Old Title",
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, nonOwnerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.Forbidden, result.CodeResult);
        Assert.Contains("Only owner or linked user with TodoList can update TodoList", result.Message);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_FailedToUpdate_ReturnsError()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var newTitle = "Updated Title";

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Old Title",
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error wile updating TodoList", result.Message);
    }

    [Fact]
    public async Task UpdateTodoListUseCaseTests_UpdateUpdatesTitleAndUpdatedAt()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var newTitle = "Updated Title";
        var originalUpdatedAt = DateTime.UtcNow.AddDays(-1);

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = newTitle,
            CurrentUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Old Title",
            OwnerId = ownerId,
            UpdatedAt = originalUpdatedAt,
            Shares = new List<TodoListShare>()
        };

        TodoList updatedList = null;

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                      .Callback<TodoList, CancellationToken>((list, ct) => updatedList = list)
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedList);
        Assert.Equal(newTitle, updatedList.Title);
        Assert.NotEqual(originalUpdatedAt, updatedList.UpdatedAt);
        Assert.True(updatedList.UpdatedAt > originalUpdatedAt);
    }
}