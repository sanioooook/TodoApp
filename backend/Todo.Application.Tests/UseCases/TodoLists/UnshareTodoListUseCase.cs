namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.UnshareTodoList;

public class UnshareTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UnshareTodoListUseCase _useCase;

    public UnshareTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new UnshareTodoListUseCase(_mockRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_UnshareByOwner_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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

        var mockUser = new User
        {
            Id = targetUserId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockUser);

        _mockRepository.Setup(x => x.RemoveShareAsync(listId, targetUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully unshared TodoList", result.Message);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_UnshareOwner_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_UnshareNonExistingList_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
        {
            ListId = listId,
            CurrentUserId = ownerId,
            TargetUserId = targetUserId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((TodoList)null);

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User());

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("TodoList not found", result.Message);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_UnshareNonSharedUser_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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

        var mockUser = new User
        {
            Id = targetUserId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockUser);

        _mockRepository.Setup(x => x.RemoveShareAsync(listId, targetUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Contains("Successfully unshared TodoList", result.Message);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_UnshareByNonOwnerAndNotSharedUser_ReturnsForbidden()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User());

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.Forbidden, result.CodeResult);
        Assert.Contains("TodoList can't be unshared because current user not owner or linked", result.Message);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_TargetUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((User)null);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("Target User not found", result.Message);
    }

    [Fact]
    public async Task UnshareTodoListUseCaseTests_FailedToUnshare_ReturnsError()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
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

        var mockUser = new User
        {
            Id = targetUserId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockTodoList);

        _mockUserRepository.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockUser);

        _mockRepository.Setup(x => x.RemoveShareAsync(listId, targetUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while unsharing TodoList", result.Message);
    }
}