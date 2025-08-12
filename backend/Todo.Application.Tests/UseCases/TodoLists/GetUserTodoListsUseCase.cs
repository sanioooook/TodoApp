namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.GetUserTodoLists;

public class GetUserTodoListsUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly GetUserTodoListsUseCase _useCase;

    public GetUserTodoListsUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _useCase = new GetUserTodoListsUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task GetUserTodoListsUseCaseTests_GetLists_ReturnsCorrectCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserTodoListsQuery
        {
            UserId = userId,
            Skip = 0,
            Take = 10
        };

        var mockLists = new List<TodoList>
            {
                new TodoList { Id = Guid.NewGuid(), Title = "List 1", OwnerId = userId },
                new TodoList { Id = Guid.NewGuid(), Title = "List 2", OwnerId = userId },
                new TodoList { Id = Guid.NewGuid(), Title = "List 3", OwnerId = userId }
            };

        _mockRepository.Setup(x => x.GetForUserAsync(userId, query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockLists);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.All(result, x => Assert.Equal(userId, x.OwnerId));
    }

    [Fact]
    public async Task GetUserTodoListsUseCaseTests_GetListsWithSkipAndTake_ReturnsCorrectRange()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserTodoListsQuery
        {
            UserId = userId,
            Skip = 1,
            Take = 2
        };

        var allLists = Enumerable.Range(1, 5).Select(i => new TodoList
        {
            Id = Guid.NewGuid(),
            Title = $"List {i}",
            OwnerId = userId
        }).ToList();

        var expectedLists = allLists.Skip(1).Take(2).ToList();

        _mockRepository.Setup(x => x.GetForUserAsync(userId, query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedLists);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, x => Assert.StartsWith("List ", x.Title));
    }

    [Fact]
    public async Task GetUserTodoListsUseCaseTests_GetLists_ReturnsCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserTodoListsQuery
        {
            UserId = userId,
            Skip = 0,
            Take = 10
        };

        var now = DateTime.UtcNow;
        var mockLists = new List<TodoList>
            {
                new TodoList
                {
                    Id = Guid.NewGuid(),
                    Title = "Test List 1",
                    OwnerId = userId,
                    CreatedAt = now.AddDays(-2),
                    UpdatedAt = now.AddDays(-1)
                },
                new TodoList
                {
                    Id = Guid.NewGuid(),
                    Title = "Test List 2",
                    OwnerId = userId,
                    CreatedAt = now.AddDays(-1),
                    UpdatedAt = now
                }
            };

        _mockRepository.Setup(x => x.GetForUserAsync(userId, query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockLists);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());

        var resultList = result.ToList();
        Assert.Equal("Test List 1", resultList[0].Title);
        Assert.Equal("Test List 2", resultList[1].Title);
        Assert.Equal(userId, resultList[0].OwnerId);
        Assert.Equal(userId, resultList[1].OwnerId);
    }
}