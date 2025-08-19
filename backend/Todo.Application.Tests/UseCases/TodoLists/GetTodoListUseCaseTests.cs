namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.GetTodoList;

public class GetTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly GetTodoListUseCase _useCase;

    public GetTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _useCase = new GetTodoListUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task GetTodoListUseCaseTests_GetExistingList_ReturnsSuccess()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var query = new GetTodoListQuery
        {
            UserId = userId,
            ListId = listId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Test List",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow,
            Shares = new List<TodoListShare>()
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.HaveResult);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Equal(listId, result.TodoListDto.Id);
        Assert.Equal(mockTodoList.Title, result.TodoListDto.Title);
        Assert.Equal(mockTodoList.OwnerId, result.TodoListDto.OwnerId);
        Assert.Equal(mockTodoList.CreatedAt, result.TodoListDto.CreatedAt);
        Assert.Equal(mockTodoList.UpdatedAt, result.TodoListDto.UpdatedAt);
        Assert.Empty(result.TodoListDto.SharedWithUsers);
    }

    [Fact]
    public async Task GetTodoListUseCaseTests_GetNonExistingList_ReturnsNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetTodoListQuery
        {
            UserId = userId,
            ListId = listId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList)null);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.HaveResult);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("TodoList not found", result.Message);
    }

    [Fact]
    public async Task GetTodoListUseCaseTests_GetListWithShares_ReturnsShares()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var sharedUserId1 = Guid.NewGuid();
        var sharedUserId2 = Guid.NewGuid();

        var query = new GetTodoListQuery
        {
            UserId = userId,
            ListId = listId
        };

        var mockTodoList = new TodoList
        {
            Id = listId,
            Title = "Test List",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow,
            Shares = new List<TodoListShare>
            {
                new TodoListShare { UserId = sharedUserId1 },
                new TodoListShare { UserId = sharedUserId2 }
            }
        };

        _mockRepository.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTodoList);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.HaveResult);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Equal(2, result.TodoListDto.SharedWithUsers.Count());
        Assert.Contains(result.TodoListDto.SharedWithUsers, x => x.UserId == sharedUserId1);
        Assert.Contains(result.TodoListDto.SharedWithUsers, x => x.UserId == sharedUserId2);
    }
}