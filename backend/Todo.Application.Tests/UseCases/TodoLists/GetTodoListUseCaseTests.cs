namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.GetTodoList;

public class GetTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly GetTodoListUseCase _useCase;

    public GetTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _useCase = new GetTodoListUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_Lists_For_User()
    {
        var ownerId = Guid.NewGuid();
        var query = new GetTodoListQuery { ListId = Guid.NewGuid(), UserId = ownerId };
        var list = new TodoList { Id = query.ListId, OwnerId = ownerId, Title = "Shared" };


        _repoMock.Setup(r => r.GetByIdAsync(query.ListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}