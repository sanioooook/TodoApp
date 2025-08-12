namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.ShareTodoList;

public class ShareTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly ShareTodoListUseCase _useCase;

    public ShareTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _useCase = new ShareTodoListUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareByOwner_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.AddShareAsync(listId, targetUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully share TodoList", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareOwnerWithSelf_ReturnsBadRequest()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = ownerId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("TodoList can't be shared with owner", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareWithAlreadySharedUser_ReturnsBadRequest()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId,
            Shares = new List<TodoListShare>
                {
                    new TodoListShare { UserId = targetUserId }
                }
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("TodoList has already shared with user", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareNonExistingList_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((TodoList)null);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("TodoList not found", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareByNonOwnerAndNotSharedUser_ReturnsForbidden()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = nonOwnerId,
            TargetUserId = targetUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
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
        Assert.Contains("TodoList can't be shared because current user not owner or linked", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_ShareWithMaxShares_ReturnsBadRequest()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId,
            Shares = new List<TodoListShare>
                {
                    new TodoListShare { UserId = Guid.NewGuid() },
                    new TodoListShare { UserId = Guid.NewGuid() },
                    new TodoListShare { UserId = Guid.NewGuid() }
                }
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("TodoList can't be shared. This list already shared 3 times", result.Message);
    }

    [Fact]
    public async Task ShareTodoListUseCaseTests_FailedToShare_ReturnsError()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new ShareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            OwnerId = ownerId,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockRepository.Setup(x => x.AddShareAsync(listId, targetUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while sharing TodoList", result.Message);
    }
}