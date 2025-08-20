namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.GetUserTodoLists;

public class GetUserTodoListsUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly GetUserTodoListsUseCase _useCase;

    public GetUserTodoListsUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _useCase = new GetUserTodoListsUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_Lists_For_User()
    {
        var userId = Guid.NewGuid();
        var lists = new List<TodoList>
        {
            new() { Id = Guid.NewGuid(), OwnerId = userId, Title = "List1" },
            new() { Id = Guid.NewGuid(), OwnerId = userId, Title = "List2" }
        };

        _repoMock.Setup(r => r.GetForUserAsync(userId, 0, 10, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lists);

        var result = await _useCase.HandleAsync(new GetUserTodoListsQuery { UserId = userId, Skip = 0, Take = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}